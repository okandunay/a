using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Port.Entities.Entities
{
    [Table("PortLog")]
    [BsonIgnoreExtraElements]
    public  class PortLog
    {
        [BsonElement("Id")]
        public int Id { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
