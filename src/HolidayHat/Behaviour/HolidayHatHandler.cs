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
        SendAll(RpcTarget.All);
    }
    
    private void OnDestroy()
    {
        HolidayHatPlugin.DefaultPlayerHatIndex.SettingChanged -= SyncDefaultHatIndex;
    }

    private void SyncDefaultHatIndex(object sender, EventArgs e)
    {
        HolidayHatPlugin.Log.LogInfo(
            $"Sending our Default Hat IDX ([{HolidayHatPlugin.DefaultPlayerHatIndex.Value}]) to everyone...");
        
        photonView.RPC(nameof(RPC_SyncDefaultHat), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.DefaultPlayerHatIndex.Value);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        SendAll(newPlayer);
    }

    private void SendAll(RpcTarget target)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HolidayHatPlugin.Log.LogInfo(
                $"Sending the room's startup time ({(HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate):M}) to everyone...");

            // If the master client don't have a room date, then it IS the original master. Otherwise, ownership has changed and we have to use the original room data
            photonView.RPC(nameof(RPC_SyncRoomStartupTimestamp),
                target,
                (HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate).Ticks);
        }
        
        HolidayHatPlugin.Log.LogInfo(
            $"Sending our Default Hat IDX ([{HolidayHatPlugin.DefaultPlayerHatIndex.Value}]) to everyone...");

        photonView.RPC(nameof(RPC_SyncDefaultHat), target, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.DefaultPlayerHatIndex.Value);
        
        HolidayHatPlugin.Log.LogInfo($"Sending our Boot Date ({HolidayHatPlugin.ClientDate:M}) to everyone...");

        photonView.RPC(nameof(RPC_SyncBootDate), target, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.ClientDate.Year,
            HolidayHatPlugin.ClientDate.Month, 
            HolidayHatPlugin.ClientDate.Day);
        
    }
    
    private void SendAll(Photon.Realtime.Player target)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            HolidayHatPlugin.Log.LogInfo(
                $"Sending the room's startup time ({(HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate):M}) to {target.NickName}...");

            // If the master client don't have a room date, then it IS the original master. Otherwise, ownership has changed and we have to use the original room data
            photonView.RPC(nameof(RPC_SyncRoomStartupTimestamp), target,
                (HolidayHatPlugin.RoomDate ?? HolidayHatPlugin.ClientDate).Ticks);
        }
        
        HolidayHatPlugin.Log.LogInfo(
            $"Sending our Default Hat IDX ([{HolidayHatPlugin.DefaultPlayerHatIndex.Value}]) to {target.NickName}...");

        photonView.RPC(nameof(RPC_SyncDefaultHat), target, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.DefaultPlayerHatIndex.Value);
        
        HolidayHatPlugin.Log.LogInfo($"Sending our Boot Date ({HolidayHatPlugin.ClientDate:M}) to {target.NickName}...");

        photonView.RPC(nameof(RPC_SyncBootDate), target, 
            PhotonNetwork.LocalPlayer.ActorNumber,
            HolidayHatPlugin.ClientDate.Year,
            HolidayHatPlugin.ClientDate.Month,
            HolidayHatPlugin.ClientDate.Day);
        
    }
    
    public override void OnJoinedRoom()
    {
        SendAll(RpcTarget.All);
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
                $"Received new Default Hat IDX of {p.NickName}: [{defaultHatIndex}]!");
            
            HolidayHatPlugin.UpdatePlayerDefaultHatIndex(p, defaultHatIndex);
            RefreshHolidayHats(p, $"because we received {p.NickName}'s new Default Hat IDX : [{defaultHatIndex}]");
        }
        else
        {
            HolidayHatPlugin.Log.LogError($"Can't receive Default Hat IDX of  player {actorNumber}!");
        }
    }

    private static void RefreshHolidayHats(Photon.Realtime.Player p, string reason = "")
    {
        var ch = PlayerHandler.GetPlayerCharacter(p);

        if (ch != null &&
            ch.refs.customization is { } c
           )
        {
            if (
                GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(p) is { } data && 
                data.customizationData.currentHat == HolidayHatPlugin.HolidayHatCosmeticIndex)
            {
                HolidayHatPlugin.Log.LogDebug(
                    $"Updating character {ch.characterName}'s hats {reason}...");

                HolidayHatPatcher.UpdateHolidayHat(c, data, p);
                HolidayHatPlugin.UpdatePlayerDummies(p);
            }
            else
            {
                HolidayHatPlugin.Log.LogInfo(
                    $"{p.NickName} is not wearing the Holiday Hat. No need to update...");
            }
        }
        else
        {
            if (ch == null)
            {
                HolidayHatPlugin.Log.LogDebug($"{p.NickName}'s character doesn't exist yet.");
            }
            else
            {
                HolidayHatPlugin.Log.LogError($"Can't update {p.NickName}'s hats!");
            }
        }
    }
    
    [PunRPC]
    public void RPC_SyncBootDate(int actorNumber, int year, int month, int day)
    {
        if (PhotonNetwork.TryGetPlayer(actorNumber, out var p))
        {
            var playerBootDate = new DateTime(year, month, day);
            
            HolidayHatPlugin.Log.LogInfo(
                $"Received Boot Date for {p.NickName}: ({playerBootDate:M})");
            
            HolidayHatPlugin.UpdatePlayerBootDate(p, playerBootDate);
            RefreshHolidayHats(p, $"because we received {p.NickName}'s Boot Date : {playerBootDate:M}");
        }
        else
        {
            HolidayHatPlugin.Log.LogError($"Player {actorNumber} doesn't exists.");
        }
    }
}