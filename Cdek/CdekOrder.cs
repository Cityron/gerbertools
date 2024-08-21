using System.Collections.Generic;

namespace CdekService.Models
{
    public class Order
    {
        public string Number { get; set; }
        public string Comment { get; set; }
        public DeliveryRecipientCost DeliveryRecipientCost { get; set; }
        public List<DeliveryRecipientCostAdv> DeliveryRecipientCostAdv { get; set; }
        public Location FromLocation { get; set; }
        public Location ToLocation { get; set; }
        public List<Package> Packages { get; set; }
        public Recipient Recipient { get; set; }
        public Sender Sender { get; set; }
        public List<Service> Services { get; set; }
        public int TariffCode { get; set; }
    }

    public class DeliveryRecipientCost
    {
        public decimal Value { get; set; }
    }

    public class DeliveryRecipientCostAdv
    {
        public decimal Sum { get; set; }
        public decimal Threshold { get; set; }
    }

    public class Location
    {
        public string Code { get; set; }
        public string FiasGuid { get; set; }
        public string PostalCode { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string City { get; set; }
        public string KladrCode { get; set; }
        public string Address { get; set; }
    }

    public class Package
    {
        public string Number { get; set; }
        public string Comment { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public decimal Width { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string WareKey { get; set; }
        public Payment Payment { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int Amount { get; set; }
        public decimal Weight { get; set; }
        public string Url { get; set; }
    }

    public class Payment
    {
        public decimal Value { get; set; }
    }

    public class Recipient
    {
        public string Name { get; set; }
        public List<Phone> Phones { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
    }

    public class Sender
    {
        public string Name { get; set; }
    }

    public class Service
    {
        public string Code { get; set; }
    }
}