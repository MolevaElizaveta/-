using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseData.Models
{
    [Table("orgs")]
    public class Organization
    {
        public static BigInteger OrgCounter = BigInteger.Zero;

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public BigInteger OrgId { get; set; }

        [Column("orgname")]
        public string OrgName { get; set; }

        public ObservableCollection<Warehouse> warehouses { get; set; } 
            = new ObservableCollection<Warehouse>();

        public Organization(string name)
        {
            OrgId = ++OrgCounter;
            OrgName = name;
        }

    }
}
