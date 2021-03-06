using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
    public class TalkModel
    {
        public int TalkId { get; set; }
       
        public string Title { get; set; }
      
        public string Abstract { get; set; }
  
        public int Level { get; set; }

        public SpeakerModel Speaker { get; set; }
    }
}