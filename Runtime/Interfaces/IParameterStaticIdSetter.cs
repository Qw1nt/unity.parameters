namespace Parameters.Runtime.Interfaces
{
    public interface IParameterStaticIdSetter
    {
        public int Id { get; }
        
        void SetStaticId(int id);
    }
}