using System;
using System.Collections.Generic;
using Catalyst.App;
using Catalyst.Http;
using Catalyst;
using System.Linq;
using System.IO;

namespace Demo
{
	public enum ProjectType
    {
        Site,
        App,
        Something
    }

    public enum Gender
    {
        Male,
        Female
    }

	public class Project : Model
    {
		public int Aantal;
		public string Name;
		public DateTime Start;
		public ProjectType Type;
		public Gender Gender;
		public string Whateves;
		public ProjectType[] Types;
		public string Description;
		public ICollection<Task> Tasks = new List<Task>();
        public Location Plaats = new Location();

		public Project()
        {
			validator = new Validator(v =>
            {
                v.ErrorIf(Name.IsEmpty(), "name", "Name should not be empty");
                v.ErrorIf(Plaats.Lattitude != Plaats.Longitude, "plaats", "Lats hihger than long");
            });
        }
	 }

    public class Task : Model
    {
		public bool Active;
		public string Name;
		public Location Location = new Location();
		public ProjectType[] Types;
		public ICollection<SubTask> SubTasks = new List<SubTask>();

        public Task()
        {
			validator = new Validator(v =>
            {
                v.ErrorIf(Name.IsEmpty(), "name", "Name should not be empty");
            });
        }
    }

	public class SubTask : Model 
	{
		public string Owner;
	}

   public class AdminApp : App
    {
        public AdminApp ()
		{
			Get ("/", request => {
				var project = GetProject ();
				project.Name = "hello";
				return this.View("views/admin/index", project);
			});
				
			Post ("/", request => {
				var postedProject = request.Params ("project");
				var project = GetProject ();

				project.UpdateWithHash (postedProject);
				if(!project.Validate()) 				
					return this.View ("views/admin/index", project);

				return "Ok, all is updated :-)";
			});
        }
		
		private static Project GetProject()
        {
            return new Project()
            {
				Tasks = {  
					new Task {}, 
					new Task { 
						SubTasks = { 
							new SubTask { }, 
							new SubTask { } ,
							new SubTask { } ,
							new SubTask { }  } 
					} 
				}
            };
        }
	}
}

