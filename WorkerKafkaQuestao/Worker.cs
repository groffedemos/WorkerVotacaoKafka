using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using WorkerKafkaQuestao.Data;
using WorkerKafkaQuestao.Kafka;

namespace WorkerKafkaQuestao
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly VotacaoRepository _repository;
        private readonly string _topico;
        private readonly string _groupId;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public Worker(ILogger<Worker> logger, IConfiguration configuration,
            VotacaoRepository repository)
        {
            _logger = logger;
            _configuration = configuration;
            _repository = repository;
            _topico = _configuration["ApacheKafka:Topic"];
            _groupId = _configuration["ApacheKafka:GroupId"];
            _consumer = KafkaExtensions.CreateConsumer(configuration);
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Topic = {_topico}");
            _logger.LogInformation($"Group Id = {_groupId}");
            _logger.LogInformation("Aguardando mensagens...");
            _consumer.Subscribe(_topico);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    var result = _consumer.Consume(stoppingToken);
                    var eventData = result.Message.Value;
                    
                    _logger.LogInformation(
                        $"[{_groupId} | Nova mensagem] " +
                        eventData);

                    ProcessEvent(eventData, result.Partition.Value);
                });
           }           
        }

        private void ProcessEvent(string eventData, int partition)
        {
            _logger.LogInformation($"[{DateTime.Now:HH:mm:ss} Evento] " + eventData);

            QuestaoEventData questaoEventData = null;
            try
            {
                questaoEventData = JsonSerializer.Deserialize<QuestaoEventData>(
                    eventData, _jsonSerializerOptions);
            }
            catch
            {
                _logger.LogError(
                    "Erro durante a deserializacao dos dados recebidos!");
            }

            if (questaoEventData is not null)
            {
                if (!String.IsNullOrWhiteSpace(questaoEventData.IdVoto) &
                    !String.IsNullOrWhiteSpace(questaoEventData.Horario) &
                    !String.IsNullOrWhiteSpace(questaoEventData.Tecnologia) &
                    !String.IsNullOrWhiteSpace(questaoEventData.Instancia))
                {
                    questaoEventData.Topic = _topico;
                    questaoEventData.ConsumerGroup = _groupId;
                    questaoEventData.Partition = partition;
                    _repository.SaveVotoTecnologia(questaoEventData);
                    _logger.LogInformation($"Voto = {questaoEventData.IdVoto} | " +
                        $"Tecnologia = {questaoEventData.Tecnologia} | " +
                        "Evento computado com sucesso!");
                    return;
                }

                _logger.LogError($"Formato dos dados do evento inválido!");
            }
        }
    }
}