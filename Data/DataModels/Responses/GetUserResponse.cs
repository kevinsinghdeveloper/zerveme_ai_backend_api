namespace zervemedata.Data.DataModels.Responses
{
    using zervemedata.Data.Enumerations;
    
    public class GetUserResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the provided answer was correct.
        /// </summary>
        public string userName { get; set; }
        
        public string firstName { get; set; }

        public string lastName { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the provided answer was correct.
        /// </summary>
        public string emailAddress { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the provided answer was correct.
        /// </summary>
        public Role? role { get; set; }
        
    } 
}

