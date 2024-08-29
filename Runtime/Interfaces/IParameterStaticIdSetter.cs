namespace Parameters.Runtime.Interfaces
{
    public interface IParameterStaticIdSetter
    {
        public ulong Id { get; }
        
        void SetStaticId(ulong id);
    }
}