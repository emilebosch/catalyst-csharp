using System;
using Catalyst;

namespace Demo
{
	public class UserRegistrationFlow : StateFlow
    {
        public UserRegistrationFlow()
        {
            WhenInState("start", () =>
            {
                OnEntering(() =>
                {
                    SendEmail("userregistration/please_confirm", on_confirm: "user_confirmed");
                    SetTimeout(TimeSpan.FromDays(10), on_timeout: "timeout");

                    TransitionTo("waiting_for_confirmation_or_timeout", because: "the user needs to valdate his account");
                    WaitForEvents();
                });
            });

            WhenInState("waiting_for_confirmation_or_timeout", () =>
            {
				OnEvent("resend_email", data => 
				{
					Log("Resend the email, and wait for a event");
                    SendEmail("please_confirm", on_confirm: "user_confirmed");
				});
                OnEvent("user_confirmed", data => TransitionTo("user_confirmed", because: "the user has confirmed directly! Kudos!"));
                OnEvent("timeout", data =>
                {
                    SendEmail("please_confirm", on_confirm: "user_confirmed");
                    SetTimeout(TimeSpan.FromDays(10), on_timeout: "timeout");

                    TransitionTo("waiting_for_confirmation_after_second_warning", because: "the user failed to confirm after 10 days");
                    WaitForEvents();
                });
            });

            WhenInState("waiting_for_confirmation_after_second_warning", () =>
            {
                OnEvent("user_confirmed", data => TransitionTo("user_confirmed", because: "the user has confirmed after the second warning. Pfew!"));
                OnEvent("timeout", data =>
                {
                    TransitionTo("abort_user_account", because: "the user stil hasn't confirmed after 20 days :(");
                });
            });

            WhenInState("abort_user_account", () =>
            {
                OnEntering(() =>
                {
                    Log("Sending the user an e-mail, to say that his account has been removed");
                    Log("Start the call the user why u no account");
                    Log("Aborting the account");

                    SetCompleted();
                });
            });

            WhenInState("user_confirmed", () =>
            {
                OnEntering(() =>
                {
                    ClearTimeouts();
                    Log("Thanks for signing up!");
                    Log("Start the adoption");

                    SetCompleted();
                });
            });
        }

        private void ClearTimeouts()
        { 
			StateApp.Timeouts.RemoveAll(a=>a.StateId == State.Id);
        }

        public void SendEmail(string name, string on_confirm)
        {

        }

        public void SetTimeout(TimeSpan span, string on_timeout)
        {
			StateApp.Timeouts.Add(new Timeout {EventName=on_timeout,StateId=State.Id, Time=DateTime.Now.Add(span)});
        }
    }
}

