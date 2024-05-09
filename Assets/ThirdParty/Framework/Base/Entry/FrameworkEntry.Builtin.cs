namespace Framework
{
    public partial class FrameworkEntry : MonoSingletonScript<FrameworkEntry>
    {
        private static FsmManager _fsm;
        private static ProcedureManager _procedure;
        private static DataNodeManager _dataNode;
        private static EventManager _event;
        private static ObjectPoolManager _objectPool;
        private static SoundManager _soundManager;
        private static ResourceManager _resourceManager;
        private static UIManager _uiManager;
        private static LuaManager _luaManager;

        public static FsmManager Fsm
        {
            get
            {
                if (_fsm == null)
                {
                    _fsm = GetManager<FsmManager>();
                }

                return _fsm;
            }
        }

        public static ProcedureManager Procedure
        {
            get
            {
                if (_procedure == null)
                {
                    _procedure = GetManager<ProcedureManager>();
                }

                return _procedure;
            }
        }

        public static DataNodeManager DataNode
        {
            get
            {
                if (_dataNode == null)
                {
                    _dataNode = GetManager<DataNodeManager>();
                }

                return _dataNode;
            }
        }

        public static EventManager Event
        {
            get
            {
                if (_event == null)
                {
                    _event = GetManager<EventManager>();
                }

                return _event;
            }
        }

        public static ObjectPoolManager ObjectPool
        {
            get
            {
                if (_objectPool == null)
                {
                    _objectPool = GetManager<ObjectPoolManager>();
                }

                return _objectPool;
            }
        }

        public static SoundManager Sound
        {
            get
            {
                if(_soundManager == null)
                {
                    _soundManager = GetManager<SoundManager>();
                }
                return _soundManager;
            }
        }

        public static ResourceManager Resource
        {
            get
            {
                if(_resourceManager == null)
                {
                    _resourceManager = GetManager<ResourceManager>();
                }
                return _resourceManager;
            }
        }

        public static UIManager UI
        {
            get
            {
                if (_uiManager == null)
                {
                    _uiManager = GetManager<UIManager>();
                }
                return _uiManager;
            }
        }

        public static LuaManager Lua
        {
            get
            {
                if (_luaManager == null)
                {
                    _luaManager = GetManager<LuaManager>();
                }
                return _luaManager;
            }
        }
    }
}