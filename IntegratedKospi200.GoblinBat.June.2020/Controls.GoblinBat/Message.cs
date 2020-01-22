namespace ShareInvest.Controls
{
    internal partial class Message
    {
        internal string Exists
        {
            get
            {
                return "Statistics don't Exist.\n\nPlease Try Again.";
            }
        }
        internal string Unreliable
        {
            get
            {
                return "The Statistics are Very Unreliable.\n\nThus there is No Strategy to Recommend.\n\nPlan Strategy or Stop Trading.";
            }
        }
        internal string Confident
        {
            get
            {
                return "Find Most Confident Strategy. . .\n\nClick the\n\n'Cancel'\n\nButton to Manually Create a Strategy.";
            }
        }
        internal string Continue
        {
            get
            {
                return "\n\nDo You Want to Continue with BackTesting??\n\nIf You don't Want to Proceed,\nPress 'No'.\n\nAfter 35 Seconds the Program is Terminated.";
            }
        }
    }
}