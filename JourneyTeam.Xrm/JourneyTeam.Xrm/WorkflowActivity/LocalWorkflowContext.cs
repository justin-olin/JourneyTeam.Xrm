﻿using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace JourneyTeam.Xrm.WorkflowActivity
{
    public class LocalWorkflowContext
    {
        /// <summary>
        /// Microsoft Dynamics CRM organization service factory
        /// </summary>
        private readonly IOrganizationServiceFactory _serviceFactory;

        public CodeActivityContext ExecutionContext { get; set; }

        /// <summary>
        ///     Workflow context
        /// </summary>
        public IWorkflowContext Context { get; }

        /// <summary>
        ///     Microsoft Dynamics CRM organization service.
        /// </summary>
        public IOrganizationService OrganizationService { get; private set; }

        /// <summary>
        ///     Provides logging run-time trace information for plug-ins. 
        /// </summary>
        public ITracingService TracingService { get; }

        public LocalWorkflowContext(CodeActivityContext activityContext)
        {
            if (activityContext == null)
            {
                throw new ArgumentNullException(nameof(activityContext));
            }

            // Obtain the activity execution context
            ExecutionContext = activityContext;

            // Obtain the execution context service from activity context
            Context = activityContext.GetExtension<IWorkflowContext>();

            // Obtain the tracing service from the activity context
            TracingService = activityContext.GetExtension<ITracingService>();

            // Obtain the organization factory service from the activity context
            _serviceFactory = activityContext.GetExtension<IOrganizationServiceFactory>();

            // Use the factory to generate the organization service.
            OrganizationService = _serviceFactory.CreateOrganizationService(Context.UserId);
        }

        /// <summary>
        /// Create CRM Organization Service for a specific user id
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>CRM Organization Service</returns>
        /// <remarks>Useful for impersonation</remarks>
        public IOrganizationService CreateOrganizationService(Guid userId)
        {
            return _serviceFactory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// Writes a trace message to the CRM trace log.
        /// </summary>
        /// <param name="message">Message name to trace.</param>
        public void Trace(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
            {
                return;
            }

            TracingService.Trace(Context == null
                ? message
                : $"{message}, Correlation Id: {Context.CorrelationId}, Initiating User: {Context.InitiatingUserId}");
        }
    }
}
