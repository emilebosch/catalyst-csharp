using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace Catalyst
{
	public class Log
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }

    public class StateLog
    {
        public string State { get; set; }
        public int Id { get; set; }
        public string Trigger { get; set; }
        public MachineState StartState { get; set; }
        public object ReturnedHostAction { get; set; }
        public List<Log> Log { get; set; }

        public StateLog()
        {
            Log = new List<Log>();
        }
    }

    public class MachineState
    {
        public string Id { get; set; }
        public string StateName { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsAwaitingEvents { get; set; }
        public bool IsError { get; set; }
    }

    public class StateDefinition
    {
        public string Name { get; set; }
        public Action Start { get; set; }
        public Dictionary<string, Action<object>> Events { get; set; }

        public StateDefinition()
        {
            Events = new Dictionary<string, Action<object>>();
        }
    }

    public class SystemState
    {
        public MachineState CurrentState { get; set; }
        public List<StateLog> Journey { get; set; }
    }

    public class StateFlow
    {
        Dictionary<string, StateDefinition> states = new Dictionary<string, StateDefinition>();
        StateDefinition currentStateDefinition;
        public List<StateLog> Logs { get; set; }
        public Action<Log> Logger;

        protected void WhenInState(string statename, Action dsl)
        {
            currentStateDefinition = new StateDefinition();
            currentStateDefinition.Name = statename;
            dsl();
            states.Add(statename, currentStateDefinition);
            currentStateDefinition = null;
        }

        protected void Log(string entiy, params object[] p)
        {
            var log = new Log { Message = String.Format(entiy, p), Time = DateTime.Now };
            currentLog.Log.Add(new Log { Message = String.Format(entiy, p), Time = DateTime.Now }); 
            if (Logger != null)
                Logger(log);
        }

        protected void SetCompleted()
        {
            State.IsCompleted = true;
        }

        protected void OnEntering(Action t)
        {
            currentStateDefinition.Start = t;
        }
      
        public SystemState GetSystemState()
        {
            return new SystemState { Journey = this.Logs, CurrentState = this.State };
        }

        public void SetSystemState(SystemState obj)
        {
            Logs = obj.Journey;
            State = obj.CurrentState;
        }

        protected void OnEvent(string name, Action<object> t)
        {
            currentStateDefinition.Events.Add(name, t);
        }

        protected void TransitionTo(string state, string because=null)
        {
            Log("TRANSITION: From '" + State.StateName + "' -> '" + state +"'"+ (because != null ? " because " + because : string.Empty));
            
            State.StateName = state;
            State.IsCompleted = false;
        }

        protected void TransitionTo(string state, bool when, string because=null)
        {
            if (when)
                TransitionTo(state, because);
        }

        public void EmitEvent(string name, object data = null)
        {
			if (!State.IsAwaitingEvents)
                throw new Exception("Can't set an event because this state is not waiting for any");

            if (!states[State.StateName].Events.ContainsKey(name))
                throw new Exception("No event for current state with name:" + name);

            Log("EVENT: {0} with data '{1}'", name, data);

            CreateLogIfNotExist();
            currentLog.Trigger = "Event";

            State.IsAwaitingEvents = false;
            states[State.StateName].Events[name](data);
            Run();
        }

		public void SetState(int id) 
		{
            State = Logs.First(a => a.Id == id).StartState;
			Logs.Clear();
		}

        public void Run()
        {
            while (!State.IsAwaitingEvents && !State.IsCompleted && !State.IsError)
                Step();
        }

        StateLog currentLog;

        public void Step()
        {
            if (String.IsNullOrEmpty(State.StateName))
                State.StateName = "start";

            var definition = GetState(State.StateName);
            if (definition.Start != null)
            {
                CreateLogIfNotExist();  
                try
                {
                    currentLog.Trigger = "OnEntering";
                    definition.Start();
                }
                catch (Exception exception)
                {
                    Log("Error raised: '"+exception.Message+"'");
                    State.IsError = true;
                }
            }

            if (definition.Events.Count() > 0)
            {
                WaitForEvents();
            }

            if (PostStep != null)
                PostStep();
        }

        private void CreateLogIfNotExist()
        {
            if (Logs == null)
            {
                Logs = new List<StateLog>();
            }

            var snapshot = JsonConvert.DeserializeObject( JsonConvert.SerializeObject(State), State.GetType()); 
            currentLog = new StateLog();
            currentLog.State = State.StateName;
            currentLog.StartState = snapshot as MachineState;
            currentLog.Id = Logs.Count(); 
            Logs.Add(currentLog);
        }

        public Action PostStep;

        protected void WaitForEvents()
        {
            State.IsAwaitingEvents = true;
        }

        public StateDefinition GetState(string name)
        {
            return states[State.StateName];
        }

        public MachineState State { get; set; }

        public string Journey()
        {
            var str = new StringWriter();

            var writer = new JsonTextWriter(str);
            writer.Formatting = Formatting.Indented;
            writer.QuoteName = false; 

            var jsonSer = new JsonSerializer();
            jsonSer.NullValueHandling = NullValueHandling.Ignore;
            jsonSer.Serialize(writer, GetSystemState());
            Console.WriteLine(str.ToString());
			return str.ToString();
        }
    }

    public class MachineDebugger
    {
        StateFlow machine = null;
        public MachineDebugger(StateFlow machine)
        {
            this.machine = machine;
        }

        public bool Start()
        {
            while (true)
            {
                machine.Journey();
                if (machine.State.IsAwaitingEvents)
                {
                    HandleAwaitingEvents();
                }
                else
                {
                    Console.WriteLine("Replay from journey #id or type quit");
                    var snapshotId = Console.ReadLine();
                    if (snapshotId == "quit")
                        return false;

                    var snapshot = machine.Logs.First(a => a.Id == Int16.Parse(snapshotId));
                    machine.State = snapshot.StartState;
                    machine.Logs.Clear();
                    machine.Run();
                } 
            }
        }

        private void HandleAwaitingEvents()
        {
            if (machine.State.IsAwaitingEvents)
            {
                Console.WriteLine("Looks like this state is waiting for events");
                var availableEvents = machine.GetState(machine.State.StateName).Events;
                int i = 0;

                foreach (var evt in availableEvents)
                    Console.WriteLine(i++ + " - " + evt.Key);

                Console.WriteLine("Tell which event to execute (by number)");

                var index = int.Parse(Console.ReadLine());
                var picked = availableEvents.ElementAt(index);

                Console.WriteLine("Add extra event data");
                machine.EmitEvent(picked.Key, Console.ReadLine());
            }
        }
    }
}