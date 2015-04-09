using System.IO;

namespace cpt2xml
{
    /// <summary>
    /// Inerface for messsage show warper
    /// </summary>
    interface IOutputMessageWrap
    {
        /// <summary>
        /// Show text message
        /// </summary>
        /// <param name="message">Message text</param>
        public void Show(string message);
    }
}