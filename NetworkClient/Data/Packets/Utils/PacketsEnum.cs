
using SocketClient;
using System;
using System.Collections.Generic;
using System.Text;
using static SocketClient.ClientOptions;

namespace Packets.Utils
{
    /// <summary>
    /// Входящие индификаторы пакетов
    /// </summary>
    public enum InputPacketsEnum : ushort
    {
        #region Profile

        /// <summary>
        /// Ответ на запрос криптографического ключа
        /// </summary>
        SecurityAESResult = 1,

        /// <summary>
        /// Ответ на запрос о авторизации
        /// </summary>
        LogInResult,

        /// <summary>
        /// Сообщение о том что на аккаунт зашел кто-то другой
        /// </summary>
        AnotherAuth,

        /// <summary>
        /// Ответ на запрос о восстановлении сессии
        /// </summary>
        RecoverySessionResult,

        /// <summary>
        /// Ответ на запрос данных аккаунта
        /// </summary>
        ProfileDataResult,

        /// <summary>
        /// Ответ на запрос данных информации о пользователе
        /// </summary>
        ProfileInfoDataResult,

        #endregion

        #region Data

        /// <summary>
        /// Ответ на запрос данных о доступных типах персонажей
        /// </summary>
        CharacterDataResult,

        /// <summary>
        /// Ответ на запрос данных конфигурации сервера
        /// </summary>
        ConfigDataResult,

        /// <summary>
        /// Ответ на запрос данных о доступных вещах
        /// </summary>
        ItemDataResult,

        /// <summary>
        /// Ответ запрос данных о доступных картах
        /// </summary>
        MapDataResult,

        #endregion

        #region Sandbox

        /// <summary>
        /// Ответ на запрос о создании комнаты
        /// </summary>
        SandboxCreateResult,

        /// <summary>
        /// Ответ на запрос о подключение к комнате
        /// </summary>
        SandboxConnectResult,

        /// <summary>
        /// Сообщение о отключении игрока с песочницы
        /// </summary>
        SandboxDisconnect,

        /// <summary>
        /// Ответ на запрос комнат в песочнице
        /// </summary>
        SandboxRoomListResult,

        /// <summary>
        /// Ответ на запрос данных комнаты в песочнице
        /// </summary>
        SandboxRoomResult,

        /// <summary>
        /// Ответ на запрос игроков в комнате в песочнице
        /// </summary>
        SandboxUserInfoListResult,

        /// <summary>
        /// Сообщение о том что была создана комната
        /// </summary>
        SandboxCreate,

        /// <summary>
        /// Сообщение о том что была удалена комната
        /// </summary>
        SandboxRemove,

        /// <summary>
        /// Изменение кол-ва игроков в комнате
        /// </summary>
        SandboxChangePlayerCount,

        #endregion

        #region Shootout

        /// <summary>
        /// Ответ на запрос о создании комнаты на выбывание
        /// </summary>
        ShootoutCreateResult,

        /// <summary>
        /// Ответ на запрос о подключении к комнате на выбывание
        /// </summary>
        ShootoutConnectResult,

        /// <summary>
        /// Ответ на запрос о получении списка комнат
        /// </summary>
        ShootoutRoomListResult,

        /// <summary>
        /// Ответ на запрос списка игроков в комнате
        /// </summary>
        ShootoutPlayerListResult,

        /// <summary>
        /// Сообщение о том что была создана комната
        /// </summary>
        ShootoutCreate,

        /// <summary>
        /// Сообщение о том что была удалена комната
        /// </summary>
        ShootoutRemove,

        /// <summary>
        /// Сообщение о том что подключился игрок в комнату
        /// </summary>
        ShootoutConnect,

        /// <summary>
        /// Сообщение о том что отключился игрок от комнаты
        /// </summary>
        ShootoutDisconnect,

        /// <summary>
        /// Сообщение о том что игрок изменил комманду в комнате
        /// </summary>
        ShootoutChangeTeam,

        /// <summary>
        /// Изменение кол-ва игроков в комнате
        /// </summary>
        ShootoutChangePlayerCount,

        #endregion

        #region FastFight

        /// <summary>
        /// Ответ на запрос готовых к быстрой битве игроков 
        /// </summary>
        FastFightReadyList,

        /// <summary>
        /// Ответ на запрос случайного противника (не найдено)
        /// </summary>
        FastFightRandomNoFind,

        /// <summary>
        /// Пакет с запросом на быструю битву
        /// </summary>
        FastFightRequest,

        /// <summary>
        /// Ответ на запрос на быструю битву
        /// </summary>
        FastFightRequestResult,

        /// <summary>
        /// Ответ на запрос на быструю битву (время ожидания истекло)
        /// </summary>
        FastFightRequestTimeOut,

        /// <summary>
        /// Пакет с данными о отключении игрока от быстрого боя
        /// </summary>
        FastFightDisconnect,

        /// <summary>
        /// Пакет с данными о подключении игрока к быстрому бою
        /// </summary>
        FastFightReady,

        #endregion

        #region Tournament

        TournamentListResult,

        TournamentConnectResult,

        TournamentConnectionInfo,

        #endregion

        #region Character

        /// <summary>
        /// Ответ на запрос о создании персонажа
        /// </summary>
        CharacterCreateResult,

        /// <summary>
        /// Ответ на запрос о установке основного персонажа
        /// </summary>
        CharacterEquipResult,

        /// <summary>
        /// Ответ на запрос списка персонажей на аккаунте
        /// </summary>
        CharacterListResult,

        /// <summary>
        /// Ответ на запрос удаления персонажа с аккаунта
        /// </summary>
        CharacterRemoveResult,

        #endregion

        #region Item

        /// <summary>
        /// Ответ на запрос одевания вещи
        /// </summary>
        ItemEquipResult,

        /// <summary>
        /// Ответ на запрос списка вещей в инвентаре
        /// </summary>
        ItemInventoryListResult,

        /// <summary>
        /// Ответ на запрос продажи вещи в инвентаре
        /// </summary>
        ItemSellResult,

        /// <summary>
        /// Ответ на запрос улучшении вещи в инвентаре
        /// </summary>
        ItemUpgradeResult,

        /// <summary>
        /// Ответ на запрос восстановления вещи в инвентаре
        /// </summary>
        ItemRepairResult,

        /// <summary>
        /// Сообщение с данные о новой вещи в инвентаре
        /// </summary>
        InventoryItem,

        #endregion

        #region Shop

        /// <summary>
        /// Ответ на запрос списка вещей в магазине
        /// </summary>
        ShopListResult,

        /// <summary>
        /// Ответ на запрос покупки вещи в магазине
        /// </summary>
        ShopBuyItemResult,

        /// <summary>
        /// Ответ на запрос покупки серебра в магазине
        /// </summary>
        ShopBuyDenariusResult,

        #endregion

        #region Other

        /// <summary>
        /// Сообщение о том что сервер занят
        /// </summary>
        RoomServerBusyResult,

        /// <summary>
        /// Сообщение с новыми данными валюты
        /// </summary>
        UpdateCoinsResult,

        #endregion

        #region Utils
        Ping
        #endregion

    }

    /// <summary>
    /// Исходящие идентификаторы пакетов
    /// </summary>
    public enum OutputPacketsEnum : ushort
    {
        #region Profile

        /// <summary>
        /// Запрос на данные о криптографических ключах
        /// </summary>
        SecurityRSAPublicKey = 1,

        /// <summary>
        /// Запрос на авторизацию
        /// </summary>
        LogIn,

        /// <summary>
        /// Запрос на восстановление сессии
        /// </summary>
        RecoverySession,

        /// <summary>
        /// Запрос данных аккаунта
        /// </summary>
        ProfileData,

        #endregion

        #region Data

        /// <summary>
        /// Запрос данных о доступных типов персонажей
        /// </summary>
        CharacterData,

        /// <summary>
        /// Запрос данных конфигураций сервера
        /// </summary>
        ConfigData,

        /// <summary>
        /// Запрос данных о доступных итемах
        /// </summary>
        ItemData,

        /// <summary>
        /// Запрос данных о доступных картах
        /// </summary>
        MapData,

        #endregion

        #region Sandbox

        /// <summary>
        /// Запрос на подключение в список рассылки комнат
        /// </summary>
        SandboxReady,

        /// <summary>
        /// Запрос на создание комнаты
        /// </summary>
        SandboxCreate,

        /// <summary>
        /// Запрос на подключение к комнате
        /// </summary>
        SandboxConnect,

        /// <summary>
        /// Запрос списка участников в комнате
        /// </summary>
        SandboxUserInfoList,

        #endregion

        #region Shootout

        /// <summary>
        /// Запрос на создание комнаты на выбывание 
        /// </summary>
        ShootoutCreate,

        /// <summary>
        /// Запрос на подключение к комнате на выбывание
        /// </summary>
        ShootoutConnect,

        /// <summary>
        /// Запрос на отключение от комнаты на выбывание
        /// </summary>
        ShootoutDisconnect,

        /// <summary>
        /// Запрос на подключение в список рассылки комнат
        /// </summary>
        ShootoutReady,

        /// <summary>
        /// Запрос списка персонажей в комнате
        /// </summary>
        ShootoutUserInfoList,

        /// <summary>
        /// Запрос на смену команды
        /// </summary>
        ShootoutChangeTeam,

        #endregion

        #region FastFight

        /// <summary>
        /// Установка доступности игрока
        /// </summary>
        FastFightReady,

        /// <summary>
        /// Запрос на битву в быстром бою
        /// </summary>
        FastFightRequest,

        /// <summary>
        /// Запрос на поиск случайного противника в быстром бою
        /// </summary>
        FastFightRandomRequest,

        /// <summary>
        /// Ответ на вызов в быстром бою
        /// </summary>
        FastFightResponse,

        #endregion

        #region Tournament

        TournamentList,

        TournamentConnect,

        TournamentDisconnect,

        #endregion

        #region Character

        /// <summary>
        /// Запрос на создания персонажа
        /// </summary>
        CharacterCreate,

        /// <summary>
        /// Запрос на установку текущего персонажа
        /// </summary>
        CharacterEquip,

        /// <summary>
        /// Запрос списка персонажей на аккаунте
        /// </summary>
        CharacterList,

        /// <summary>
        /// Запрос на удаление персонажа с текущего аккаунта
        /// </summary>
        CharacterRemove,

        #endregion

        #region Item

        /// <summary>
        /// Запрос на одевание итема
        /// </summary>
        ItemEquip,

        /// <summary>
        /// Запрос данных инвентаря на текущем аккаунте
        /// </summary>
        ItemInventoryList,

        /// <summary>
        /// Запрос на продажу вещи с инвентаря на текущем аккаунте
        /// </summary>
        ItemSell,

        /// <summary>
        /// Запрос на улучшение вещи в инвентаре на текущем аккаунте
        /// </summary>
        ItemUpgrade,

        /// <summary>
        /// Запрос на восстановление вещи в инвентаре на текущем аккаунте
        /// </summary>
        ItemRepair,

        #endregion

        #region Shop

        /// <summary>
        /// Запрос списка вещей в магазине
        /// </summary>
        ShopItemList,

        /// <summary>
        /// Запрос на покупку вещи в магазине
        /// </summary>
        ShopBuyItem,

        /// <summary>
        /// Запрос на покупку серебра в магазине
        /// </summary>
        ShopBuyDinarius,

        #endregion
    }

    public static class AddEnumPacket
    {
        /// <summary>
        /// Добавить пакет для обработки сервером
        /// </summary>
        /// <param name="packetId">Индификатор пакета в системе</param>
        /// <param name="packet">Обработчик пакета</param>
        /// <returns></returns>
        public static bool AddPacket(this ClientOptions options, InputPacketsEnum packetId, PacketHandle packet)
        {
            var r = options.Packets.ContainsKey((ushort)packetId);
            if (!r)
                options.Packets.Add((ushort)packetId, packet);
            return !r;
        }
    }
}
