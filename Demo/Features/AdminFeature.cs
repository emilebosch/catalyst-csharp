using Catalyst;
using Catalyst.Http.Test;

public class Ok : Feature 
{
	public Ok() 
	{
		Scenario("Test the admin interface", ()=>
		{
			Driver.Visit("/admin");		
			
			Driver.Fill("Aantal", with: "20");
			Driver.Check("Active");
			Driver.Pick("Female");									
			Driver.Fill("Description", with: "this is a test");
			Driver.Fill("Name", with: "this is a test");
			Driver.Fill("Start", with: "07/07/1982");					
			Driver.Select("Type", with: "Something");
			Driver.Submit();	
			
			Driver.Text(doesntContain: "error");
			Driver.Text (contains: "Yay");
		});
	}
}
