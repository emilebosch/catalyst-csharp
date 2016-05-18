using System.Collections.Generic;
using System;
using Catalyst.App;
using Catalyst;

namespace Demo
{
	public class Worklog 
	{
		public int Id;
		public DateTime Date;
		public int Hours;
		public string Description;
		public int TaskId;
	}
	
	public class Worktask 
	{
		public int Id;
		public string Description;
	}

	public class Timeout 
	{
		public string StateId;
		public DateTime Time;
		public string EventName;
	}

	public class WorklogApp : App
	{
		List<Worklog> logs = new List<Worklog>();
		
		public WorklogApp()
		{
			Post("/create", request => 
			{
				var worklog = request.Json<Worklog>();
				worklog.Description = worklog.Description + DateTime.Now;
				logs.Add(worklog);

				return this.Json(worklog);
			});
		}
	}
}

