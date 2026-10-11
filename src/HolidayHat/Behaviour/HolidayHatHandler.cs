using System;
using HolidayHat.Patchers;
using Photon.Pun;

namespace HolidayHat.Behaviour;

public class HolidayHatHandler: MonoBehaviourPunCallbacks
{
    public static HolidayHatHandler instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        HolidayHatPlugin.DefaultPlayerHatIndex.SettingChanged += SyncDefaultHatIndex;
    }


    public override void OnCreatedRoom()
    {
        // Send the room's timestamp to the new player
        if (PhotonNetwork.IsMasterClient)
        {
            HolidayHatPlugin.Log.LogInfo(
                $"Setting up the room's startup time ({(HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate)})...");

            // If the master client don't have a room date, then it IS the original master. Otherwise, ownership has changed and we have to use the original room data
            photonView.RPC(nameof(RPC_SyncRoomStartupTimestamp),
                RpcTarget.All,
                (HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate).Ticks);
        }
    }
    private void OnDestroy()
    {
        HolidayHatPlugin.DefaultPlayerHatIndex.SettingChanged -= SyncDefaultHatIndex;
    }

    private void SyncDefaultHatIndex(object sender, EventArgs e)
    {
        photonView.RPC(nameof(RPC_SyncDefaultHat), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.DefaultPlayerHatIndex.Value);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // Send the room's timestamp to the new player
        if(PhotonNetwork.IsMasterClient)
        {
            HolidayHatPlugin.Log.LogInfo(
                $"Sending the room's startup time ({(HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate)}) to {newPlayer.NickName}...");
            
            // If the master client don't have a room date, then it IS the original master. Otherwise, ownership has changed and we have to use the original room data
            photonView.RPC(nameof(RPC_SyncRoomStartupTimestamp), 
                newPlayer,
                (HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate).Ticks);
        }
        
        // Send our default hat index to the new player

        HolidayHatPlugin.Log.LogInfo($"Sending our default hat index ({HolidayHatPlugin.DefaultPlayerHatIndex.Value}) to {newPlayer.NickName}...");
        
        photonView.RPC(nameof(RPC_SyncDefaultHat), 
            newPlayer, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.DefaultPlayerHatIndex.Value);
    }

    public override void OnJoinedRoom()
    {
        HolidayHatPlugin.Log.LogInfo(
            $"Sending our default hat index ({HolidayHatPlugin.DefaultPlayerHatIndex.Value}) to everyone...");
        
        photonView.RPC(nameof(RPC_SyncDefaultHat), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, HolidayHatPlugin.DefaultPlayerHatIndex.Value);
    }

    [PunRPC]
    public void RPC_SyncRoomStartupTimestamp(long timestamp)
    {
        HolidayHatPlugin.Log.LogInfo(
            $"Received the room's startup timestamp ({timestamp})!");

        HolidayHatPlugin.ReceiveTimestamp(timestamp);
    }
    
    [PunRPC]
    public void RPC_SyncDefaultHat(int actorNumber, int defaultHatIndex)
    {
        if (PhotonNetwork.TryGetPlayer(actorNumber, out var p))
        {
            HolidayHatPlugin.Log.LogInfo(
                $"Received default hat idx ({defaultHatIndex}) for {p.NickName}!");
            
            HolidayHatPlugin.UpdatePlayerDefaultHatIndex(p, defaultHatIndex);
            
            if (GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(p) is { } data)
            {
                if (PlayerHandler.GetPlayerCharacter(p) is { } c)
                {
                    HolidayHatPatcher.UpdateHolidayHat(c, data);
                    HolidayHatPlugin.UpdatePlayerDummies(p);
                }
            }
            else
            {

                HolidayHatPlugin.Log.LogError($"Can't update {p}'s hats!");
            }
        }
        else
        {
            HolidayHatPlugin.Log.LogError($"Can't receive default hat index for player {actorNumber}!");
        }
    }
}