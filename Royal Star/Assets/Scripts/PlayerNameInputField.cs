using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;

//giving a proper namespace to your script prevents clashes with other assets and developers.
namespace Com.ESGI.RoyalStar
{
    // Player name input field.
    // Let the user input his name, will appear above the player in the game.
    //We first make sure that this script enforce the InputField
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        // Store the PlayerPref Key to avoid typos
        const string playerNamePrefKey = "PlayerName";
        
        // MonoBehaviour method called on GameObject by Unity during initialization phase.
        private void Start()
        {
            //PlayerPrefs.HasKey(), PlayerPrefs.GetString() and PlayerPrefs.SetString():
            //PlayerPrefs is a simple lookup list of paired entries (like an excel sheet with two columns),
            //one is the key, one is the Value. The Key is a string, and is totally arbitrary, you decide how to name
            //and you will need to stick to it throughout the development. Because of that, it make sense to always store
            //your PlayerPrefs Keys in one place only, a convenient way is to use a [Static| variable declaration, because
            //it won't change over time during the game and is the same everytime.
            //One could go all the way and declare it const, but this is something you'll get into as you gain more and more
            //experience with C#, this is just teasing with C# scope of possibilities here.
            
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
                
                //PhotonNetwork.NickName:
                //This is main point of this script, setting up the name of the player over the network.
                //The script uses this in two places, once during Start() after having check if the name
                //was stored in the PlayerPrefs, and inside the public method SetPlayerName(). Right now,
                //nothing is calling this method, we need to bind the InputField OnValueChange() to call
                //SetPlayerName() so that every time the user is editing the InputField, we record it. We
                //could do this only when the user is pressing play, this is up to you, however this is a bit
                //more involving script wise, so let's keep it simple for the sake of clarity. It also means
                //that no matter what the user will do, the input will be remembered, which is often the desired behavior.
                PhotonNetwork.NickName =  defaultName;
            }
        }

        public void SetPlayerName(string value)
        {   
            //We verify the content of value
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey,value);
        }
        
    }
}
