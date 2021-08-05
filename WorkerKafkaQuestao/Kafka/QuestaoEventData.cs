namespace WorkerKafkaQuestao.Kafka
{
    public class QuestaoEventData
    {
        public string IdVoto { get; set; }
        public string Horario { get; set; }
        public string Instancia { get; set; }
        public string Topic { get; set; }
        public int Partition { get; set; }

        public string Tecnologia { get; set; }
        public string ConsumerGroup { get; set; }
    }
}