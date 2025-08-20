

using HealthMonitoringContracts;

namespace AdminToolsConsoleApp.HelperMethods
{
    public interface IExecutor
    {
        /// <summary>
        /// Adds a new student to the student service.
        /// </summary>
        /// <param name="emailService">The student service to add the student to.</param>
        void AddEmail(IAdminAlertEmails emailService);

        /// <summary>
        /// Lists all students from the student service.
        /// </summary>
        /// <param name="emailService">The student service to retrieve students from.</param>
        void ListAllEmails(IAdminAlertEmails emailService);

       


        /// <summary>
        /// Removes a student from the student service.
        /// </summary>
        /// <param name="emailService">The student service to remove the student from.</param>
        void RemoveEmail(IAdminAlertEmails emailService);
    }
}
