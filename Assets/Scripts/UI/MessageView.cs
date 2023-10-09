using Models;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text messageText;
        
        public void Initialize(MessageModel messageModel)
        {
            speakerText.text = messageModel.SpeakerNameText;
            messageText.text = messageModel.MessageText;
        }
    }
}
