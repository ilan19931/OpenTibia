﻿using OpenTibia.Common;
using OpenTibia.Common.Objects;
using OpenTibia.Common.Structures;
using OpenTibia.Data.InMemory.Contexts;
using OpenTibia.Data.Models;
using OpenTibia.Data.MsSql.Contexts;
using OpenTibia.Data.MySql.Contexts;
using OpenTibia.Data.Oracle.Contexts;
using OpenTibia.Data.PostgreSql.Contexts;
using OpenTibia.Data.Sqlite.Contexts;
using OpenTibia.FileFormats.Dat;
using OpenTibia.FileFormats.Otb;
using OpenTibia.FileFormats.Otbm;
using OpenTibia.FileFormats.Xml.Houses;
using OpenTibia.FileFormats.Xml.Items;
using OpenTibia.FileFormats.Xml.Monsters;
using OpenTibia.FileFormats.Xml.Npcs;
using OpenTibia.FileFormats.Xml.Spawns;
using OpenTibia.Game.Commands;
using OpenTibia.Game.Common.ServerObjects;
using OpenTibia.Game.Events;
using OpenTibia.Network.Sockets;
using OpenTibia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using House = OpenTibia.Common.Objects.House;
using Item = OpenTibia.Common.Objects.Item;
using Tile = OpenTibia.Common.Objects.Tile;

namespace OpenTibia.Game.Common
{
    public class Server : IServer
    {
        public Server()
        {
            dispatcher = new Dispatcher();

            scheduler = new Scheduler(dispatcher);

            MessageCollectionFactory = new MessageCollectionFactory();

            ClientFactory = new ClientFactory(this);

            DatabaseFactory = new DatabaseFactory(this, builder =>
            {
                switch (Config.DatabaseType)
                {
                    case "sqlite":

                        return new SqliteContext(Config.DatabaseOverrideConnectionString ?? ("Data Source=" + PathResolver.GetFullPath(Config.DatabaseSource) ), builder.Options);

                    case "mysql":

                        return new MySqlContext(Config.DatabaseOverrideConnectionString ?? ("Server=" + Config.DatabaseHost + ";Port=" + Config.DatabasePort + ";Database=" + Config.DatabaseName + ";User=" + Config.DatabaseUser + ";Password=" + Config.DatabasePassword + ";"), builder.Options);

                    case "mssql":

                        return new MsSqlContext(Config.DatabaseOverrideConnectionString ?? ("Server=" + Config.DatabaseHost + ";Database=" + Config.DatabaseName + ";User Id=" + Config.DatabaseUser + ";Password=" + Config.DatabasePassword + ";TrustServerCertificate=True;"), builder.Options);

                    case "postgresql":

                        return new PostgreSqlContext(Config.DatabaseOverrideConnectionString ?? ("Server=" + Config.DatabaseHost + ";Port=" + Config.DatabasePort + ";Database=" + Config.DatabaseName + ";User Id=" + Config.DatabaseUser + ";Password=" + Config.DatabasePassword + ";"), builder.Options);

                    case "oracle":

                        return new OracleContext(Config.DatabaseOverrideConnectionString ?? ("Data Source=" + Config.DatabaseHost + ";User Id=" + Config.DatabaseUser + ";Password=" + Config.DatabasePassword + ";") );

                    case "memory":

                        return new InMemoryContext(builder.Options);

                    default:

                        throw new NotImplementedException("File config.lua parameter server.database.type must be sqlite, mysql, mssql, postgresql, oracle or memory.");
                }
            } );

            Statistics = new ServerStatistics();

            Logger = new Logger(new ConsoleLoggerProvider(), LogLevel.Debug);

            PathResolver = new PathResolver();

            Randomization = new Randomization();

            Clock = new Clock(DateTime.Now.Hour, DateTime.Now.Minute);

            RateLimiting = new RateLimiting(this);

            WaitingList = new WaitingList(this);

            Channels = new ChannelCollection(this);

            RuleViolations = new RuleViolationCollection();

            Guilds = new GuildCollection();

            Parties = new PartyCollection();

            Tradings = new TradingCollection();

            NpcTradings = new NpcTradingCollection();

            Combats = new CombatCollection();

            GameObjectPool = new GameObjectPool(this);

            GameObjects = new GameObjectCollection();

            GameObjectComponents = new GameObjectComponentCollection();

            GameObjectEventHandlers = new GameObjectEventHandlerCollection();

            CommandHandlers = new CommandHandlerCollection();

            EventHandlers = new EventHandlerCollection();

            PositionalEventHandlers = new PositionalEventHandlerCollection(this);

            LuaScripts = new LuaScriptCollection(this);

            PluginLoader = new PluginLoader(this);

            Values = new Values(this);

            Config = new Config(this);

            Features = new Features(this);

            Quests = new QuestCollection(this);

            Outfits = new OutfitCollection(this);

            Mounts = new MountCollection(this);

            Vocations = new VocationCollection(this);

            Plugins = new PluginCollection(this);

            Scripts = new ScriptCollection(this);

            GameObjectScripts = new GameObjectScriptCollection(this);

            ItemFactory = new ItemFactory(this);

            PlayerFactory = new PlayerFactory(this);

            MonsterFactory = new MonsterFactory(this);

            NpcFactory = new NpcFactory(this);

            Map = new Map(this);

            Spawns = new SpawnCollection(this);

            Raids = new RaidCollection(this);

            Pathfinding = new Pathfinding(Map);
        }

        ~Server()
        {
            Dispose(false);
        }

        public string ServerName
        {
            get
            {
                return "MTOTS";
            }
        }

        public Version ServerVersion
        {
            get
            {
                return new Version(1, 9, 1);
            }
        }

        private Dispatcher dispatcher;

        private Scheduler scheduler;

        private Listener loginServer;

        private Listener gameServer;

        private Listener infoServer;

        public ServerStatus Status { get; private set; }

        public IMessageCollectionFactory MessageCollectionFactory { get; set; }

        public IClientFactory ClientFactory { get; set; }

        public IDatabaseFactory DatabaseFactory { get; set; }

        public IServerStatistics Statistics { get; set; }

        public ILogger Logger { get; set; }

        public IPathResolver PathResolver { get; set; }

        public IRandomization Randomization { get; set; }

        public Clock Clock { get; set; }

        public IRateLimiting RateLimiting { get; set; }

        public IWaitingList WaitingList { get; set; }

        public IChannelCollection Channels { get; set; }

        public IRuleViolationCollection RuleViolations { get; set; }

        public IGuildCollection Guilds { get; set; }

        public IPartyCollection Parties { get; set; }

        public ITradingCollection Tradings { get; set; }

        public INpcTradingCollection NpcTradings { get; set; }

        public ICombatCollection Combats { get; set; }

        public IGameObjectPool GameObjectPool { get; set; }

        public IGameObjectCollection GameObjects { get; set; }

        public IGameObjectComponentCollection GameObjectComponents { get; set; }

        public IGameObjectEventHandlerCollection GameObjectEventHandlers { get; set; }

        public ICommandHandlerCollection CommandHandlers { get; set; }

        public IEventHandlerCollection EventHandlers { get; set; }

        public IPositionalEventHandlerCollection PositionalEventHandlers { get; set; }

        public ILuaScriptCollection LuaScripts { get; set; }

        public IPluginLoader PluginLoader { get; set; }

        public IValues Values { get; set; }

        public IConfig Config { get; set; }

        public IFeatures Features { get; set; }

        public IQuestCollection Quests { get; set; }

        public IOutfitCollection Outfits { get; set; }

        public IMountCollection Mounts { get; set; }

        public IVocationCollection Vocations { get; set; }

        public IPluginCollection Plugins { get; set; }

        public IScriptCollection Scripts { get; set; }

        public IGameObjectScriptCollection GameObjectScripts { get; set; }

        public IItemFactory ItemFactory { get; set; }

        public IPlayerFactory PlayerFactory { get; set; }

        public IMonsterFactory MonsterFactory { get; set; }
        
        public INpcFactory NpcFactory { get; set; }

        public IMap Map { get; set; }

        public ISpawnCollection Spawns { get; set; }

        public IRaidCollection Raids { get; set; }

        public IPathfinding Pathfinding { get; set; }

        public void Start()
        {
            Statistics.Start();

            dispatcher.Start();

            scheduler.Start();

            QueueForExecution(async () =>
            {
                Logger.WriteLine(ServerName + " " + ServerVersion + " - An open Tibia server developed by mtanksl");
                Logger.WriteLine("Copyright (C) 2024 mtanksl");
                Logger.WriteLine("This program comes with ABSOLUTELY NO WARRANTY");
                Logger.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions");
                Logger.WriteLine("Source code: https://github.com/mtanksl/OpenTibia");
                Logger.WriteLine();
                            
                using (Logger.Measure("Loading lua") )
                {
                    LuaScripts.Start();
                }

#if !Target_Runtime_Linux_x64

                if ( !PathResolver.Exists("data/lualibs/mobdebug.lua") )
                {
                    Logger.WriteLine("Lua debugger is disabled due to missing data/lualibs/mobdebug.lua", LogLevel.Warning);
                }
#endif
                using (Logger.Measure("Loading dlls") )
                {
                    PluginLoader.Start();
                }

                using (Logger.Measure("Loading values config") )
                {
                    Values.Start();
                }

                using (Logger.Measure("Loading server config") )
                {
                    Config.Start();

                    Features.Start();
                }

                using (Logger.Measure("Loading channels config") )
                {
                    Channels.Start();
                }

                using (Logger.Measure("Loading quests config") )
                {
                    Quests.Start();
                }

                using (Logger.Measure("Loading outfits and mounts config") )
                {
                    Outfits.Start();

                    Mounts.Start();
                }

                using (Logger.Measure("Loading vocations config") )
                {
                    Vocations.Start();
                }
                
                using (Logger.Measure("Loading game object scripts config") )
                {
                    GameObjectScripts.Start();
                }

                using (Logger.Measure("Loading items") )
                {
                    ItemFactory.Start(OtbFile.Load(PathResolver.GetFullPath("data/items/" + Features.ClientVersion + "/items.otb") ), 
                                      DatFile.Load(PathResolver.GetFullPath("data/items/" + Features.ClientVersion + "/tibia.dat"), Features.HasFeatureFlag(FeatureFlag.SpritesUInt32), Features.HasFeatureFlag(FeatureFlag.IdleAnimations), Features.HasFeatureFlag(FeatureFlag.EnhancedAnimations), Features.HasFeatureFlag(FeatureFlag.NoMovementAnimation) ), 
                                      ItemsFile.Load(PathResolver.GetFullPath("data/items/items.xml") ) );
                }

                using (Logger.Measure("Loading monsters") )
                {
                    MonsterFactory.Start(MonsterFile.Load(PathResolver.GetFullPath("data/monsters") ) );
                }

                using (Logger.Measure("Loading npcs") )
                {
                    NpcFactory.Start(NpcFile.Load(PathResolver.GetFullPath("data/npcs") ) );
                }

                using (Logger.Measure("Loading plugins config and plugins") )
                {
                    Plugins.Start();
                }

                using (Logger.Measure("Loading scripts config and scripts") )
                {
                    Scripts.Start();
                }

                OtbmFile otbmFile;

                using (Logger.Measure("Loading map") )
                {
                    otbmFile = OtbmFile.Load(PathResolver.GetFullPath("data/world/" + Features.ClientVersion + "/" + Config.MapName + ".otbm"), ItemFactory.GetItemMetadataByOpenTibiaId);

                    Map.Start(otbmFile, 
                              HouseFile.Load(PathResolver.GetFullPath("data/world/" + Features.ClientVersion + "/" + otbmFile.MapInfo.HouseFile) ) );
                }

                if (Map.Warnings.Count > 0)
                {
                    Logger.WriteLine("Map warnings: " + string.Join(", ", Map.Warnings), LogLevel.Warning);
                }

                using (Logger.Measure("Loading spawns") )
                {
                    Spawns.Start(SpawnFile.Load(PathResolver.GetFullPath("data/world/" + Features.ClientVersion + "/" + otbmFile.MapInfo.SpawnFile) ) );

                    Raids.Start();
                }

                if (Spawns.UnknownMonsters.Count > 0)
                {
                    Logger.WriteLine("Unable to load monsters: " + string.Join(", ", Spawns.UnknownMonsters), LogLevel.Warning);
                }

                if (Spawns.UnknownNpcs.Count > 0)
                {
                    Logger.WriteLine("Unable to load npcs: " + string.Join(", ", Spawns.UnknownNpcs), LogLevel.Warning);
                }

                using (var database = DatabaseFactory.Create() )
                {
                    using (Logger.Measure("Testing database") )
                    {                    
                        if ( !database.CanConnect() )
                        {
                            Logger.WriteLine("Unable to connect to database.", LogLevel.Error);
                        }
                        else
                        {
                            if (Config.DatabaseType == "memory")
                            {
                                await database.CreateInMemoryDatabase();
                            }
                        }
                    }

                    if (Config.LoginMaxconnections > 0 && Config.LoginPort > 0)
                    {
                        using (Logger.Measure("Updating message of the day") )
                        {
                            DbMotd motd = await database.MotdRepository.GetLastMessageOfTheDay();

                            if (motd == null || motd.Message != Config.Motd)
                            {
                                database.MotdRepository.AddMessageOfTheDay(new DbMotd() { Message = Config.Motd } );

                                await database.Commit();
                            }
                        }

                        using (Logger.Measure("Updating worlds") )
                        {
                            foreach (var dbWorld in await database.WorldRepository.GetWorlds() )
                            {
                                DbWorld world = Config.Worlds
                                    .Where(w => w.Name == dbWorld.Name)
                                    .FirstOrDefault();

                                if (world != null)
                                {
                                    dbWorld.Ip = world.Ip;

                                    dbWorld.Port = world.Port;
                                }
                            }

                            await database.Commit();
                        }
                    }

                    using (Logger.Measure("Loading server storages") )
                    {
                        DbServerStorage playersPeek = await database.ServerStorageRepository.GetServerStorageByKey("PlayersPeek");

                        if (playersPeek == null)
                        {
                            playersPeek = new DbServerStorage()
                            {
                                Key = "PlayersPeek",

                                Value = "0"
                            };

                            database.ServerStorageRepository.AddServerStorage(playersPeek);

                            await database.Commit();
                        }

                        Statistics.PlayersPeek = uint.Parse(playersPeek.Value);
                    }

                    using (Logger.Measure("Loading guilds") )
                    { 
                        foreach (var dbGuild in await database.GuildRepository.GetGuilds() )
                        {
                            Guild guild = new Guild()
                            {
                                Id = dbGuild.Id,

                                Name = dbGuild.Name,

                                MessageOfTheDay = dbGuild.MessageOfTheDay,

                                Leader = dbGuild.LeaderId
                            };

                            foreach (var dbGuildMember in dbGuild.Members)
                            {
                                guild.AddMember(dbGuildMember.PlayerId, dbGuildMember.RankName);
                            }

                            foreach (var dbGuildInvitation in dbGuild.Invitations)
                            {
                                guild.AddInvitation(dbGuildInvitation.PlayerId, dbGuildInvitation.RankName);
                            }

                            Guilds.AddGuild(guild);
                        }
                    }

                    using (Logger.Measure("Loading houses") )
                    {
                        foreach (var dbHouse in await database.HouseRepository.GetHouses() )
                        {
                            House house = Map.GetHouse( (ushort)dbHouse.Id);

                            if (house != null)
                            {
                                if (dbHouse.Owner != null)
                                {
                                    house.OwnerId = dbHouse.OwnerId;

                                    house.Owner = dbHouse.Owner.Name;
                                }

                                foreach (var dbHouseAccessList in dbHouse.HouseAccessLists)
                                {
                                    if (dbHouseAccessList.ListId == 0xFE)
                                    {
                                        house.GetSubOwnersList().SetText(dbHouseAccessList.Text);
                                    }
                                    else if (dbHouseAccessList.ListId == 0xFF)
                                    {
                                        house.GetGuestsList().SetText(dbHouseAccessList.Text);
                                    }
                                    else
                                    {
                                        house.GetDoorList( (byte)dbHouseAccessList.ListId).SetText(dbHouseAccessList.Text);
                                    }
                                }

                                void AddItems(Container parent, long sequenceId)
                                {
                                    foreach (var dbHouseItem in dbHouse.HouseItems.Where(hi => hi.ParentId == sequenceId) )
                                    {
                                        Item item = ItemFactory.Create( (ushort)dbHouseItem.OpenTibiaId, (byte)dbHouseItem.Count);

                                        Serializer.DeserializeItemAttributes(item, dbHouseItem.Attributes);

                                        ItemFactory.Attach(item);

                                        if (item is Container container)
                                        {
                                            AddItems(container, dbHouseItem.SequenceId);
                                        }

                                        parent.AddContent(item);
                                    }
                                }

                                foreach (var dbHouseItem in dbHouse.HouseItems.Where(hi => ( (hi.ParentId >> 36) & 0x01) == 0x01) )
                                {
                                    int x = (int)( ( (dbHouseItem.ParentId >> 20) & 0xFFFF) );

                                    int y = (int)( ( (dbHouseItem.ParentId >> 4) & 0xFFFF) );

                                    int z = (int)( (dbHouseItem.ParentId & 0xF) );

                                    Tile tile = Map.GetTile(new Position(x, y, z) );

                                    if (tile != null)
                                    {
                                        Item item = ItemFactory.Create( (ushort)dbHouseItem.OpenTibiaId, (byte)dbHouseItem.Count);

                                        Serializer.DeserializeItemAttributes(item, dbHouseItem.Attributes);

                                        ItemFactory.Attach(item);

                                        if (item is Container container)
                                        {
                                            AddItems(container, dbHouseItem.SequenceId);
                                        }

                                        tile.AddContent(item);
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var plugin in Plugins.GetServerStartupPlugins() )
                {
                    await plugin.OnStartup();
                }

            } ).Wait();

            using (Logger.Measure("Garbage collection") )
            {
                GC.Collect();

                GC.WaitForPendingFinalizers();
            }

            List<int> ports = new List<int>();

            if (Config.LoginMaxconnections > 0 && Config.LoginPort > 0)
            {
                loginServer = new Listener(socket => new LoginConnection(this, socket) );

                loginServer.Start(Config.LoginMaxconnections, Config.LoginPort);

                ports.Add(Config.LoginPort);
            }

            if (Config.GameMaxConnections > 0 && Config.GamePort > 0)
            {
                gameServer = new Listener(socket => new GameConnection(this, socket) );

                gameServer.Start(Config.GameMaxConnections, Config.GamePort);

                ports.Add(Config.GamePort);
            }

            if (Config.InfoMaxConnections > 0 && Config.InfoPort > 0)
            {
                infoServer = new Listener(socket => new InfoConnection(this, socket) );

                infoServer.Start(Config.InfoMaxConnections, Config.InfoPort);

                ports.Add(Config.InfoPort);
            }

            Status = ServerStatus.Running;

            Logger.WriteLine("Server status: running. Client version: " + Config.ClientVersion + ". Listening on " + Config.IpAddress + " at " + string.Join(", ", ports) + ".");
        }

        private Dictionary<string, SchedulerEvent> schedulerEvents = new Dictionary<string, SchedulerEvent>();

        public void Post(Context previousContext, Action run)
        {
            DispatcherEvent dispatcherEvent = new DispatcherEvent( () =>
            {
                var start = Stopwatch.GetTimestamp();

                try
                {
                    using (var context = new Context(this, previousContext) )
                    {
                        using (var scope = new Scope<Context>(context) )
                        {
                            run();

                            context.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine(ex.ToString(), LogLevel.Error);
                }

                Statistics.IncreaseProcessingTime(Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp() ).Ticks);
            } );

            dispatcher.QueueForExecution(dispatcherEvent);
        }

        public Promise QueueForExecution(Func<Promise> run)
        {
            return Promise.Run( (resolve, reject) =>
            {
                Context previousContext = Context.Current;

                DispatcherEvent dispatcherEvent = new DispatcherEvent( () =>
                {
                    var start = Stopwatch.GetTimestamp();

                    try
                    {
                        using (var context = new Context(this, previousContext) )
                        {
                            using (var scope = new Scope<Context>(context) )
                            {
                                run().Then(resolve).Catch( (ex) =>
                                {
                                    if (ex is PromiseCanceledException)
                                    {
                                        //
                                    }
                                    else
                                    {
                                        Logger.WriteLine(ex.ToString(), LogLevel.Error);
                                    }

                                    reject(ex);
                                } );

                                context.Flush();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.ToString(), LogLevel.Error);
                                
                        reject(ex);
                    }

                    Statistics.IncreaseProcessingTime(Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp() ).Ticks);
                } );

                dispatcher.QueueForExecution(dispatcherEvent);
            } );
        }

        public Promise QueueForExecution(string key, TimeSpan executeIn, Func<Promise> run)
        {
            return Promise.Run( (resolve, reject) =>
            {
                SchedulerEvent schedulerEvent;

                if ( schedulerEvents.TryGetValue(key, out schedulerEvent) )
                {
                    schedulerEvents.Remove(key);

                    schedulerEvent.Cancel();
                }

                Context previousContext = Context.Current;

                schedulerEvent = new SchedulerEvent(executeIn, () =>
                {
                    var start = Stopwatch.GetTimestamp();

                    schedulerEvents.Remove(key);

                    try
                    {
                        using (var context = new Context(this, previousContext) )
                        {
                            using (var scope = new Scope<Context>(context) )
                            {
                                run().Then(resolve).Catch( (ex) =>
                                {
                                    if (ex is PromiseCanceledException)
                                    {
                                        //
                                    }
                                    else
                                    {
                                        Logger.WriteLine(ex.ToString(), LogLevel.Error);
                                    }

                                    reject(ex);
                                } );

                                context.Flush();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.ToString(), LogLevel.Error);
                    
                        reject(ex);
                    }

                    Statistics.IncreaseProcessingTime(Stopwatch.GetElapsedTime(start, Stopwatch.GetTimestamp() ).Ticks);
                } );

                schedulerEvent.Canceled += (sender, e) =>
                {
                    Exception ex = PromiseCanceledException.Instance;

                    reject(ex);
                };

                schedulerEvents.Add(key, schedulerEvent);

                scheduler.QueueForExecution(schedulerEvent);
            } );
        }

        public bool CancelQueueForExecution(string key)
        {
            SchedulerEvent schedulerEvent;

            if ( schedulerEvents.TryGetValue(key, out schedulerEvent) )
            {
                schedulerEvents.Remove(key);

                schedulerEvent.Cancel();

                return true;
            }

            return false;
        }

        public void ReloadPlugins()
        {
            QueueForExecution( () =>
            {
                using (Logger.Measure("Reloading plugins config and plugins") )
                {
                    try
                    {
                        var plugin = new PluginCollection(this);

                        plugin.Start();

                        Plugins.Stop();

                        Plugins.Dispose();

                        Plugins = plugin;

                        Context.Current.AddEvent(GlobalServerReloadedEventArgs.Instance);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine(ex.ToString(), LogLevel.Error);
                    }
                }

                return Promise.Completed;

            } ).Wait();
        }

        public void KickAll()
        {
            QueueForExecution( () =>
            {
                List<Player> players = new List<Player>();

                foreach (var observer in Context.Current.Server.GameObjects.GetPlayers() )
                {
                    players.Add(observer);
                }

                List<Promise> promises = new List<Promise>();

                foreach (var player in players)
                {
                    promises.Add(Context.Current.AddCommand(new CreatureDestroyCommand(player) ) );
                }

                return Promise.WhenAll(promises.ToArray() );

            } ).Wait();

            Logger.WriteLine("Kick all complete.");
        }

        public void Save()
        {
            QueueForExecution(async () =>
            {
                using (var database = DatabaseFactory.Create() )
                {
                    if (Config.GameplayAllowClones)
                    {
                        Logger.WriteLine("Saving players skipped, because file config.lua parameter server.game.gameplay.allowclones is true.", LogLevel.Warning);

                        Logger.WriteLine("Saving guilds skipped, because file config.lua parameter server.game.gameplay.allowclones is true.", LogLevel.Warning);

                        Logger.WriteLine("Saving houses skipped, because file config.lua parameter server.game.gameplay.allowclones is true.", LogLevel.Warning);
                    }
                    else
                    {
                        using (Logger.Measure("Saving players") )
                        {
                            Player[] players = GameObjectPool.GetPlayers().ToArray();

                            if (players.Length > 0)
                            {
                                DbPlayer[] dbPlayers = await database.PlayerRepository.GetPlayerByIds(players.Select(p => p.DatabasePlayerId).ToArray() );

                                foreach (var item in players.GroupJoin(dbPlayers, p => p.DatabasePlayerId, p => p.Id, (player, dbPlayers) => new { Player = player, DbPlayer = dbPlayers.FirstOrDefault() } ) )
                                {
                                    Player player = item.Player;

                                    if (player.Tile != null)
                                    {
                                        player.Spawn = player.Tile;
                                    }

                                    DbPlayer dbPlayer = item.DbPlayer;

                                    if (dbPlayer != null)
                                    {
                                        PlayerFactory.Save(dbPlayer, player);
                                    }
                                }                   
                            }
                        }

                        using (Logger.Measure("Saving guilds") )
                        {
                            Guild[] guilds = Guilds.GetGuilds().ToArray();

                            DbGuild[] dbGuilds = await database.GuildRepository.GetGuilds();

                            foreach (var id in guilds.Select(g => g.Id).Except(dbGuilds.Select(g => g.Id) ) )
                            {
                                Guild guild = guilds.Where(g => g.Id == id).First();

                                database.GuildRepository.AddGuild(new DbGuild()
                                {
                                    Name = guild.Name,

                                    MessageOfTheDay = guild.MessageOfTheDay,

                                    LeaderId = guild.Leader,

                                    Members = guild.GetMembers().Select(m => new DbGuildMember()
                                    {
                                        PlayerId = m.Key,

                                        RankName = m.Value

                                    } ).ToArray(),

                                    Invitations = guild.GetInvitations().Select(m => new DbGuildInvitation()
                                    {
                                        PlayerId = m.Key,

                                        RankName = m.Value

                                    } ).ToArray()
                                } );
                            }

                            foreach (var id in guilds.Select(g => g.Id).Intersect(dbGuilds.Select(g => g.Id) ) )
                            {
                                Guild guild = guilds.Where(g => g.Id == id).First();

                                DbGuild dbGuild = dbGuilds.Where(g => g.Id == id).First();

                                dbGuild.Name = guild.Name;

                                dbGuild.MessageOfTheDay = guild.MessageOfTheDay;

                                dbGuild.LeaderId = guild.Leader;

                                dbGuild.Members.Clear();

                                foreach (var item in guild.GetMembers() )
                                {
                                    dbGuild.Members.Add(new DbGuildMember()
                                    {
                                        PlayerId = item.Key,

                                        RankName = item.Value
                                    } );
                                }

                                dbGuild.Invitations.Clear();

                                foreach (var item in guild.GetInvitations() )
                                {
                                    dbGuild.Invitations.Add(new DbGuildInvitation()
                                    {
                                        PlayerId = item.Key,

                                        RankName = item.Value
                                    } );
                                }
                            }

                            foreach (var id in dbGuilds.Select(g => g.Id).Except(guilds.Select(g => g.Id) ) )
                            {
                                DbGuild dbGuild = dbGuilds.Where(g => g.Id == id).First();

                                database.GuildRepository.RemoveGuild(dbGuild);
                            }
                        }

                        using (Logger.Measure("Saving houses") )
                        {
                            House[] houses = Map.GetHouses().ToArray();

                            if (houses.Length > 0)
                            {
                                DbHouse[] dbHouses = await database.HouseRepository.GetHouses();

                                foreach (var item in houses.GroupJoin(dbHouses, h => h.Id, h => h.Id, (house, dbHouses) => new { House = house, DbHouse = dbHouses.FirstOrDefault() } ) )
                                {
                                    House house = item.House;

                                    DbHouse dbHouse = item.DbHouse;

                                    if (dbHouse == null)
                                    {
                                        dbHouse = new DbHouse()
                                        {
                                            Id = house.Id,

                                            OwnerId = house.OwnerId
                                        };

                                        database.HouseRepository.AddHouse(dbHouse);
                                    }
                                    else
                                    {
                                        dbHouse.OwnerId = house.OwnerId;
                                    }

                                    dbHouse.HouseAccessLists.Clear();

                                    HouseAccessList subOwnersList = house.GetSubOwnersList();

                                    if (subOwnersList.Text != null)
                                    {
                                        dbHouse.HouseAccessLists.Add(new DbHouseAccessList()
                                        {
                                            HouseId = house.Id,

                                            ListId = 0xFE,

                                            Text = subOwnersList.Text
                                        } );
                                    }

                                    HouseAccessList guestsList = house.GetGuestsList();

                                    if (guestsList.Text != null)
                                    {
                                        dbHouse.HouseAccessLists.Add(new DbHouseAccessList()
                                        {
                                            HouseId = house.Id,

                                            ListId = 0xFF,

                                            Text = guestsList.Text
                                        } );
                                    }

                                    foreach (var doorList in house.GetDoorsList() )
                                    {
                                        if (doorList.Value.Text != null)
                                        {
                                            dbHouse.HouseAccessLists.Add(new DbHouseAccessList()
                                            {
                                                HouseId = house.Id,

                                                ListId = doorList.Key,

                                                Text = doorList.Value.Text
                                            } );
                                        }
                                    }

                                    long sequenceId = 1;

                                    void AddItems(long parentId, Item item)
                                    {
                                        DbHouseItem dbHouseItem = new DbHouseItem()
                                        {
                                            HouseId = dbHouse.Id,

                                            SequenceId = sequenceId++,

                                            ParentId = parentId,

                                            OpenTibiaId = item.Metadata.OpenTibiaId,

                                            Count = item is StackableItem stackableItem ? stackableItem.Count :

                                                    item is FluidItem fluidItem ? (int)fluidItem.FluidType :

                                                    item is SplashItem splashItem ? (int)splashItem.FluidType : 1,
                                                                                    
                                            Attributes = Serializer.SerializeItemAttributes(item)
                                        };

                                        dbHouse.HouseItems.Add(dbHouseItem);

                                        if (item is Container container)
                                        {
                                            foreach (var child in container.GetItems().Reverse() )
                                            {
                                                AddItems(dbHouseItem.SequenceId, child);
                                            }
                                        }
                                    }

                                    dbHouse.HouseItems.Clear();

                                    foreach (var tile in house.GetTiles() )
                                    {
                                        foreach (var moveable in tile.GetItems().Where(i => !i.Metadata.Flags.Is(ItemMetadataFlags.NotMoveable) ).Reverse() )
                                        {
                                            AddItems( (long)0x01 << 36 | (long)tile.Position.X << 20 | (long)tile.Position.Y << 4 | (long)tile.Position.Z, moveable);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    await database.Commit();
                }

                foreach (var plugin in Plugins.GetServerSavePlugins() )
                {
                    await plugin.OnSave();
                }

            } ).Wait();

            Logger.WriteLine("Save complete.");
        }

        public void Clean()
        {
            QueueForExecution( () =>
            {
                List<Item> items = new List<Item>();

                foreach (var tile in Map.GetTiles() )
                {
                    if ( !tile.ProtectionZone)
                    {
                        foreach (var item in tile.GetItems() )
                        {
                            if ( !item.LoadedFromMap && item.ActionId == 0 && item.UniqueId == 0 && item.Metadata.Flags.Is(ItemMetadataFlags.Pickupable) && !item.Metadata.Flags.Is(ItemMetadataFlags.NotMoveable) )
                            {
                                items.Add(item);
                            }
                        }
                    }
                }

                List<Promise> promises = new List<Promise>();

                foreach (var item in items)
                {
                    promises.Add(Context.Current.AddCommand(new ItemDestroyCommand(item) ) );
                }

                return Promise.WhenAll(promises.ToArray() );

            } ).Wait();

            Logger.WriteLine("Clean complete.");
        }

        public void Pause()
        {
            if (Status == ServerStatus.Running)
            {
                Status = ServerStatus.Paused;

                Logger.WriteLine("Server status: paused.");
            }
        }

        public void Continue()
        {
            if (Status == ServerStatus.Paused)
            {
                Status = ServerStatus.Running;

                Logger.WriteLine("Server status: running.");
            }
        }

        public void Stop()
        {
            QueueForExecution(async () =>
            {
                foreach (var plugin in Plugins.GetServerShutdownPlugins() )
                {
                    await plugin.OnShutdown();
                }

                Plugins.Stop();

                Scripts.Stop();

                Spawns.Stop();

                Raids.Stop();

            } ).Wait();

            if (loginServer != null)
            {
                loginServer.Stop();
            }

            if (gameServer != null)
            {
                gameServer.Stop();
            }

            if (infoServer != null)
            {
                infoServer.Stop();
            }

            scheduler.Stop();

            dispatcher.Stop();

            Status = ServerStatus.Stopped;

            Logger.WriteLine("Server status: stopped.");
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    if (MessageCollectionFactory != null)
                    {
                        MessageCollectionFactory.Dispose();
                    }

                    if (Values != null)
                    {
                        Values.Dispose();
                    }

                    if (Config != null)
                    {
                        Config.Dispose();
                    }

                    if (Channels != null)
                    {
                        Channels.Dispose();
                    }

                    if (Quests != null)
                    {
                        Quests.Dispose();
                    }

                    if (Outfits != null)
                    {
                        Outfits.Dispose();
                    }

                    if (GameObjectScripts != null)
                    {
                        GameObjectScripts.Dispose();
                    }

                    if (Plugins != null)
                    {
                        Plugins.Dispose();
                    }

                    if (Scripts != null)
                    {
                        Scripts.Dispose();
                    }

                    if (LuaScripts != null)
                    {
                        LuaScripts.Dispose();
                    }

                    if (loginServer != null)
                    {
                        loginServer.Dispose();
                    }

                    if (gameServer != null)
                    {
                        gameServer.Dispose();
                    }

                    if (infoServer != null)
                    {
                        infoServer.Dispose();
                    }

                    if (PluginLoader != null)
                    {
                        PluginLoader.Dispose();
                    }
                }
            }
        }
    }
}