using Photon.Pun;
using UnityEngine;

namespace MyPhotonProject.Scripts
{
    public class AvatarsControllerScript : MonoBehaviour
    {
        [SerializeField]
        private AvatarExposerScript[] avatars;

        [SerializeField]
        private Transform[] startPositions;

        [SerializeField]
        private AIntentReceiver[] onlineIntentReceivers;

        [SerializeField]
        private AIntentReceiver[] offlineIntentReceivers;

        [SerializeField]
        private StartGameControllerScript startGameControllerScript;

        [SerializeField]
        private PhotonView photonView;

        [SerializeField]
        private float bumpForce;

        [SerializeField]
        private float bumpRadius;

        [SerializeField]
        private float moveForce;

        [SerializeField]
        private float yLowerLimit;

        private AIntentReceiver[] activatedIntentReceivers;

        private bool GameStarted { get; set; }

        private void Awake()
        {
            startGameControllerScript.OnlinePlayReady += ChooseAndSubscribeToOnlineIntentReceivers;
            startGameControllerScript.OfflinePlayReady += ChooseAndSubscribeToOfflineIntentReceivers;
            startGameControllerScript.PlayerJoined += ActivateAvatar;
            startGameControllerScript.PlayerLeft += DeactivateAvatar;
            startGameControllerScript.Disconnected += EndGame;
            startGameControllerScript.MasterClientSwitched += EndGame;
        }

        private void ActivateAvatar(int id)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("ActivateAvatarRPC", RpcTarget.AllBuffered, id);
            }
            else
            {
                ActivateAvatarRPC(id);
            }
        }

        private void DeactivateAvatar(int id)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("DeactivateAvatarRPC", RpcTarget.AllBuffered, id);
            }
            else
            {
                DeactivateAvatarRPC(id);
            }
        }

        private void ChooseAndSubscribeToOnlineIntentReceivers()
        {
            activatedIntentReceivers = onlineIntentReceivers;
            ResetGame();
        }

        private void ChooseAndSubscribeToOfflineIntentReceivers()
        {
            activatedIntentReceivers = offlineIntentReceivers;
            ResetGame();
        }

        private void DisableIntentReceivers()
        {
            if (activatedIntentReceivers == null)
            {
                return;
            }

            for (var i = 0; i < activatedIntentReceivers.Length; i++)
            {
                activatedIntentReceivers[i].enabled = false;
            }
        }

        private void EnableIntentReceivers()
        {
            if (activatedIntentReceivers == null)
            {
                return;
            }

            for (var i = 0; i < activatedIntentReceivers.Length; i++)
            {
                activatedIntentReceivers[i].enabled = true;
                activatedIntentReceivers[i].WantToBumpIntent = false;
                activatedIntentReceivers[i].WantToMoveLeft = false;
                activatedIntentReceivers[i].WantToMoveBack = false;
                activatedIntentReceivers[i].WantToMoveRight = false;
                activatedIntentReceivers[i].WantToMoveForward = false;
            }
        }

        private void ResetGame()
        {
            for (var i = 0; i < avatars.Length; i++)
            {
                var avatar = avatars[i];
                avatar.AvatarRigidBody.velocity = Vector3.zero;
                avatar.AvatarRigidBody.angularVelocity = Vector3.zero;
                avatar.AvatarRootTransform.position = startPositions[i].position;
                avatar.AvatarRootTransform.rotation = startPositions[i].rotation;
                avatar.AvatarRigidBodyView.enabled = activatedIntentReceivers == onlineIntentReceivers;
                avatar.AttackParticleSystem.Stop();
            }

            EnableIntentReceivers();
            GameStarted = true;
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndGame();
                return;
            }

            // If on network, only the master client can move objects
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                return;
            }

            // Do nothing if the game is not started
            if (!GameStarted)
            {
                return;
            }

            // If intents and avatars are not setup properly
            if (activatedIntentReceivers == null
                || avatars == null
                || avatars.Length != activatedIntentReceivers.Length)
            {
                Debug.LogError("There is something wrong with avatars and intents setup !");
                return;
            }

            var fallenAvatarsCount = 0;
            var activatedAvatarsCount = 0;

            for (var i = 0; i < activatedIntentReceivers.Length; i++)
            {
                var moveIntent = Vector3.zero;

                var intentReceiver = activatedIntentReceivers[i];
                var avatar = avatars[i];

                activatedAvatarsCount += avatar.AvatarRootGameObject.activeSelf ? 1 : 0;

                if (intentReceiver.WantToBumpIntent)
                {
                    for (var j = 0; j < avatars.Length; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        avatars[j]
                            .AvatarRigidBody.AddExplosionForce(bumpForce,
                                avatar.AvatarRootTransform.position,
                                bumpRadius);
                    }

                    avatar.AttackParticleSystem.Play();
                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC("PlayParticleSystemsRPC", RpcTarget.Others, i);
                    }

                    intentReceiver.WantToBumpIntent = false;
                }

                if (intentReceiver.WantToMoveBack)
                {
                    moveIntent += Vector3.back;
                }

                if (intentReceiver.WantToMoveForward)
                {
                    moveIntent += Vector3.forward;
                }

                if (intentReceiver.WantToMoveLeft)
                {
                    moveIntent += Vector3.left;
                }

                if (intentReceiver.WantToMoveRight)
                {
                    moveIntent += Vector3.right;
                }

                moveIntent = moveIntent.normalized;

                avatar.AvatarRigidBody.AddForce(moveIntent * moveForce);

                if (!(avatar.transform.position.y <= yLowerLimit))
                {
                    continue;
                }

                fallenAvatarsCount++;
            }

            if (activatedAvatarsCount - fallenAvatarsCount <= 1 && activatedAvatarsCount > 1)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            GameStarted = false;
            activatedIntentReceivers = null;

            for (var i = 0; i < avatars.Length; i++)
            {
                avatars[i].AvatarRootGameObject.SetActive(false);
            }

            startGameControllerScript.ShowMainMenu();

            DisableIntentReceivers();
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        [PunRPC]
        private void PlayParticleSystemsRPC(int avatarId)
        {
            avatars[avatarId].AttackParticleSystem.Play();
        }

        [PunRPC]
        private void ActivateAvatarRPC(int avatarId)
        {
            avatars[avatarId].AvatarRootGameObject.SetActive(true);
        }

        [PunRPC]
        private void DeactivateAvatarRPC(int avatarId)
        {
            avatars[avatarId].AvatarRootGameObject.SetActive(false);
        }
    }
}