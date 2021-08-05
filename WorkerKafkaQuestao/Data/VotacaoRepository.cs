using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper.Contrib.Extensions;
using WorkerKafkaQuestao.Kafka;

namespace WorkerKafkaQuestao.Data
{
    public class VotacaoRepository
    {
        private readonly IConfiguration _configuration;

        public VotacaoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SaveVotoTecnologia(QuestaoEventData questaoEventData)
        {
            using var conexao = new SqlConnection(
                _configuration.GetConnectionString("BaseVotacaoKafka"));
            conexao.Insert<VotoTecnologia>(new ()
            {
                IdVoto = questaoEventData.IdVoto,
                Horario = Convert.ToDateTime(questaoEventData.Horario),
                Tecnologia = questaoEventData.Tecnologia,
                Topico = questaoEventData.Topic,
                Particao = questaoEventData.Partition,
                Producer = questaoEventData.Instancia,
                Consumer = Environment.MachineName,
                ConsumerGroup = questaoEventData.ConsumerGroup
            });
        }
    }
}