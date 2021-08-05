using System;
using Dapper.Contrib.Extensions;

namespace WorkerKafkaQuestao.Data
{
    [Table("dbo.VotoTecnologia")]
    public class VotoTecnologia
    {
        [Key]
        public int Id { get; set; }
        public string IdVoto { get; set; }
        public DateTime? Horario { get; set; }
        public string Tecnologia { get; set; }
        public string Topico { get; set; }
        public int Particao { get; set; }
        public string Producer { get; set; }
        public string Consumer { get; set; }
        public string ConsumerGroup { get; set; }
    }
}