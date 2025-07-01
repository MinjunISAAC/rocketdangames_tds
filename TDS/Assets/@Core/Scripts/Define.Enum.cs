namespace Core
{
    public static partial class Define
    {
        #region [Scene]
        public enum ESceneType
        {
            Unknown = 0,
            InitializerScene = 1,
            MainScene = 2,
        }
        #endregion
        
        #region [State]
        public enum EStateType
        {
            Unknown = 0,
            Init = 1,
            Ready = 2,
            Play = 3,
            Success = 4,
            Fail = 5,
            Pause = 6,
        }
        #endregion

        #region [Resource]
        public enum ELoadType
        {
            Unknown = 0,
            Global = 1,
        }
        #endregion

        #region [UI]
        public enum ETileMoveType
        {
            Unknown = 0,
            LeftBottom_RightTop = 1,
            LeftTop_RightBottom = 2,
            RightBottom_LeftTop = 3,
            RightTop_LeftBottom = 4,
            Left_Right = 5,
            Right_Left = 6,
            Top_Bottom = 7,
            Bottom_Top = 8,
        }
        #endregion

        #region [Unit]
        public enum EUnitType
        {
            Unknown = 0,
            Type_0 = 1,
            Type_1 = 2,
            Type_2 = 3,
            Type_3 = 4,
        }
        #endregion
    }
}