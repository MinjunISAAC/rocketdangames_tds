namespace Core
{
    public static partial class Define
    {
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
    }
}