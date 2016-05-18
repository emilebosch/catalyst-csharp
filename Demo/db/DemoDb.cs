using SQLite;
using System;
using Catalyst;

namespace Demo
{	
	public class File
	{
		public File (SimpleHash hash)
		{
			hash.Update (this);
		}
		
		public File() 
		{
		}
		
		[AutoIncrement, PrimaryKey]
		public int Id { get; set; }
		public string Name { get; set; }
		public byte[] Contents {get;set;}
		public string ContentType {
			get;
			set;
		}
	}
	
	public class Order
	{
		[AutoIncrement, PrimaryKey]
		public int Id { get; set; }
		public DateTime PlacedTime { get; set; }
	}
	
	public class OrderHistory {
		[AutoIncrement, PrimaryKey]
		public int Id { get; set; }
		public int OrderId { get; set; }
		public DateTime Time { get; set; }
		public string Comment { get; set; }
	}
	
	public class OrderLine
	{
		[AutoIncrement, PrimaryKey]
		public int Id { get; set; }
        [Indexed("IX_OrderProduct", 1)]		
		public int OrderId { get; set; }
        
		[Indexed("IX_OrderProduct", 2)]
        public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public OrderLineStatus Status { get; set; }
	}
	
	public enum OrderLineStatus 
	{
		Placed = 1,
		Shipped = 100
	}

	public class DemoDb : SQLiteConnection
	{
		public DemoDb (string path) : base (path)
		{
			Trace = true;
		}
	}
}
