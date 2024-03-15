using ServiceLocator.Utilities;
/**  This script demonstrates implementation of the Observer Pattern.
  *  If you're interested in learning about Observer Pattern, 
  *  you can find a dedicated course on Outscal's website.
  *  Link: Link: https://outscal.com/courses
  **/

namespace ServiceLocator.Events
{
    public class EventService
    {
        public GameEventController<int> OnMapSelected { get; private set; }

        public EventService()
        {
            OnMapSelected = new GameEventController<int>();
        }
        
    }
}