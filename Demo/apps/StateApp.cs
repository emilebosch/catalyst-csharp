using System;
using Catalyst.App;
using Catalyst;
using System.Collections.Generic;
using Catalyst.Http;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;

namespace Demo
{
	public class StateApp : App 
	{
		List<StateFlow> running = new List<StateFlow>();
		public static List<Timeout> Timeouts = new List<Timeout>();

		public StateApp() 
		{
			Get("/start", request => 
			{
				var flow = new UserRegistrationFlow();
				running.Add(flow);
				flow.State = new MachineState { Id="5"};	
				flow.Run();
				return "ok";
			});

			Get("/show", request=> 
			{
				var machine = 
					running.First(a=>a.State.Id == request.Query("id"));

				var emit = 
					request.Query("event");

				if(emit!=null) 
					machine.EmitEvent(emit, null);

				var move_to = 
					request.Query("move_to");
			
				if(move_to!=null) 
					machine.SetState(int.Parse(move_to));				

				var html = 
					new Tag("pre", machine.Journey());

				var def = machine.GetState(machine.State.StateName);
				if(machine.State.IsAwaitingEvents) 
				{
					html += 
					new Tag("ul", from evt in def.Events select 
						new Tag("li", new Tag("a", evt.Key, href:request.Url("/show?id={0}&event={1}", machine.State.Id, evt.Key))));	
				}
				return html;
			});

			Get("/", request => 
			{
				return 
					new Tag("ul",running.Select(a => 
						new Tag("li", new Tag("a", "{0} {1} {2} {3}".Inject(a.State.StateName, a.State.IsError, a.State.IsCompleted, a.State.IsAwaitingEvents), 
					    	href: request.Url("/show?id={0}", a.State.Id)))));
			});
		}
	}	
}
