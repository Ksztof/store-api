using PerfumeStore.Domain.DTOs.Request;
using PerfumeStore.Domain.Interfaces;
using PerfumeStore.Domain.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Domain.ShippingDetails
{
    public class ShippingDet : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string HomeNumber { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public Order Order { get; set; }

        public void CreateShippingDetail(CreateOrderDtoDom createOrderDtoDom)
        {
            FirstName = createOrderDtoDom.FirstName;
            LastName = createOrderDtoDom.LastName;
            Street = createOrderDtoDom.Street;
            StreetNumber = createOrderDtoDom.StreetNumber;
            HomeNumber = createOrderDtoDom.HomeNumber;
            PostCode = createOrderDtoDom.PostCode;
            City = createOrderDtoDom.City;
            PhoneNumber = createOrderDtoDom.PhoneNumber;
        }
    }
}
