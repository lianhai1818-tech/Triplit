namespace DAL
{
    public abstract class BaseDB
    {
        protected string connectionString;

        public BaseDB(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}