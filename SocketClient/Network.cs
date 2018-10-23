using System;
using System.Collections.Generic;
using System.Text;
using Cipher.RC.RC4;
using Packets.Utils;
using SocketClient;
using SocketClient.Utils.Buffer;
using UnityEngine;

public class Network : MonoBehaviour
{
    private static Network s_instance;

    public static Network Instance
    {
        get
        {
            return s_instance;
        }
    }

    public string IpAddress = "62.109.30.64";

    public bool LocalAddress = false;

    public int Port = 4625;

    public static ClientOptions socketOptions = new ClientOptions
    {
        AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork,
        ProtocolType = System.Net.Sockets.ProtocolType.Tcp,
        ReceiveBufferSize = 81960,
        inputCipher = new XRC4Cipher("754gtvv%43"),
        outputCipher = new XRC4Cipher("YYYYYT24653"),
    };

    public static SocketClient.SocketClient networkClient;

    private const int ReconnectMaxCount = 0;

    private const int ReconnectTimeOut = 1900; //1.9s * 15 = 29.5s/30s

    private int ReconnectCount = 0;

    public void Start()
    {
        AddPackets();

        socketOptions.IpAddress = LocalAddress ? "127.0.0.1" : IpAddress;
        socketOptions.Port = Port;

        socketOptions.OnClientConnectEvent += SocketOptions_OnClientConnectEvent;
        socketOptions.OnClientDisconnectEvent += SocketOptions_OnClientDisconnectEvent;
        socketOptions.OnExtensionEvent += SocketOptions_OnExtensionEvent;
        networkClient = new SocketClient.SocketClient(socketOptions);
        s_instance = this;
    }

    private void AddPackets()
    {
        //Profile
        socketOptions.AddPacket(InputPacketsEnum.SecurityAESResult, Packets.Security.AES_SK_Public.Run);
        socketOptions.AddPacket(InputPacketsEnum.LogInResult, Packets.Profile.LogIn.Run);
        socketOptions.AddPacket(InputPacketsEnum.AnotherAuth, Packets.Profile.AnotherAuth.Run);
        socketOptions.AddPacket(InputPacketsEnum.RecoverySessionResult, Packets.Profile.RecoverySession.Run);

        socketOptions.AddPacket(InputPacketsEnum.ProfileDataResult, Packets.Profile.ProfileData.Run);

        //Data
        socketOptions.AddPacket(InputPacketsEnum.CharacterDataResult, Packets.InfoData.CharacterData.Run);
        socketOptions.AddPacket(InputPacketsEnum.ConfigDataResult, Packets.InfoData.ConfigData.Run);
        socketOptions.AddPacket(InputPacketsEnum.ItemDataResult, Packets.InfoData.ItemData.Run);
        socketOptions.AddPacket(InputPacketsEnum.MapDataResult, Packets.InfoData.MapData.Run);

        //FastFight
        socketOptions.AddPacket(InputPacketsEnum.FastFightReadyList, Packets.FastFight.FastFightReadyList.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightRandomNoFind, Packets.FastFight.FastFightRandomNoFind.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightRequest, Packets.FastFight.FastFightRequest.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightRequestResult, Packets.FastFight.FastFightRequestResult.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightRequestTimeOut, Packets.FastFight.FastFightRequestTimeOut.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightDisconnect, Packets.FastFight.FastFightDisconnect.Run);
        socketOptions.AddPacket(InputPacketsEnum.FastFightReady, Packets.FastFight.FastFightReady.Run);

        //Character
        socketOptions.AddPacket(InputPacketsEnum.CharacterCreateResult, Packets.Character.CharacterCreate.Run);
        socketOptions.AddPacket(InputPacketsEnum.CharacterEquipResult, Packets.Character.CharacterEquip.Run);
        socketOptions.AddPacket(InputPacketsEnum.CharacterListResult, Packets.Character.CharacterList.Run);
        socketOptions.AddPacket(InputPacketsEnum.CharacterRemoveResult, Packets.Character.CharacterRemove.Run);

        //Item
        socketOptions.AddPacket(InputPacketsEnum.ItemEquipResult, Packets.Item.ItemEquip.Run);
        socketOptions.AddPacket(InputPacketsEnum.ItemInventoryListResult, Packets.Item.ItemInventoryList.Run);
        socketOptions.AddPacket(InputPacketsEnum.ItemSellResult, Packets.Item.ItemSell.Run);
        socketOptions.AddPacket(InputPacketsEnum.ItemUpgradeResult, Packets.Item.ItemUpgrade.Run);
        socketOptions.AddPacket(InputPacketsEnum.ItemRepairResult, Packets.Item.ItemRepair.Run);

        //Shop
        socketOptions.AddPacket(InputPacketsEnum.ShopListResult, Packets.Shop.ShopItemList.Run);
        socketOptions.AddPacket(InputPacketsEnum.ShopBuyItemResult, Packets.Shop.ShopBuyItem.Run);
        socketOptions.AddPacket(InputPacketsEnum.ShopBuyDenariusResult, Packets.Shop.ShopBuySilver.Run);

        //Messages
        socketOptions.AddPacket(InputPacketsEnum.RoomServerBusyResult, Packets.Messages.ServerBusyMessage.Run);
        socketOptions.AddPacket(InputPacketsEnum.UpdateCoinsResult, Packets.Messages.UpdateCoinsMessage.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxCreate, Packets.Messages.SandboxCreateMessage.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxRemove, Packets.Messages.SandboxRemoveMessage.Run);
        socketOptions.AddPacket(InputPacketsEnum.InventoryItem, Packets.Messages.InventoryItemMessage.Run);

        //Sandbox
        socketOptions.AddPacket(InputPacketsEnum.SandboxCreateResult, Packets.Sandbox.SandboxCreate.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxConnectResult, Packets.Sandbox.SandboxConnect.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxDisconnect, Packets.Sandbox.SandboxDisconnect.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxRoomListResult, Packets.Sandbox.SandboxReady.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxUserInfoListResult, Packets.Sandbox.SandboxUserInfoList.Run);
        socketOptions.AddPacket(InputPacketsEnum.SandboxChangePlayerCount, Packets.Sandbox.SandboxChangePlayerCount.Run);

        socketOptions.AddPacket(InputPacketsEnum.Ping, Packets.Utils.Ping.Run);
    }

    private void SocketOptions_OnExtensionEvent(Exception ex, System.Net.Sockets.Socket s)
    {
        ThreadHelper.Instance.InvokeOnMain(() => { Debug.Log(ex.ToString()); });
    }

    private void SocketOptions_OnClientConnectEvent(NetworkClient client)
    {
        ReconnectCount = 0;
        ThreadHelper.Instance.InvokeOnMain(() => { Debug.Log("Lobby server connected"); });

        if (!client.NoReconnectDisconnect)
        {
            if (client.RefreshKeys != null && client.Session != null)
                Packets.Profile.RecoverySession.Send(client);
            else if (client.LoginName != null && client.Password != null)
                Packets.Profile.LogIn.Send(client.LoginName, client.Password);
        }
    }

    private async void SocketOptions_OnClientDisconnectEvent(NetworkClient client)
    {
        if (client == null)
            return;

        if (ReconnectMaxCount == ReconnectCount)
        {
            ThreadHelper.Instance.InvokeOnMain(() => { Debug.LogError($"Lobby server cannot connected (try:{ReconnectMaxCount})"); });
            return;
        }

        if (client.NoReconnectDisconnect)
        {
            ThreadHelper.Instance.InvokeOnMain(() => { Debug.Log($"Lobby server cannot connected (NoReconnectDisconnect:true)"); });
            return;
        }

        await System.Threading.Tasks.Task.Delay(ReconnectTimeOut);

        ThreadHelper.Instance.InvokeOnMain(() => { Debug.Log("Lobby server try reconnect"); });


        // возможно тут нужно выводить окно с попытками переподключиться
        networkClient.Connect();

        ReconnectCount++;
        // ну а тут его закрывать
    }

    private void OnApplicationQuit()
    {
        if (NetworkClient.Instance != null)
        {
            NetworkClient.Instance.NoReconnectDisconnect = true;
            NetworkClient.Instance.LoginName = null;
            NetworkClient.Instance.Password = null;
            NetworkClient.Instance.Session = null;
            NetworkClient.Instance.RefreshKeys = null;
        }

        if (networkClient != null)
            networkClient.Disconnect();
    }

}