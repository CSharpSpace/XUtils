using System;
namespace XUtils.Threading.Base
{
	public class WorkItemInfo
	{
		public bool UseCallerCallContext
		{
			get;
			set;
		}
		public bool UseCallerHttpContext
		{
			get;
			set;
		}
		public bool DisposeOfStateObjects
		{
			get;
			set;
		}
		public CallToPostExecute CallToPostExecute
		{
			get;
			set;
		}
		public PostExecuteWorkItemCallback PostExecuteWorkItemCallback
		{
			get;
			set;
		}
		public WorkItemPriority WorkItemPriority
		{
			get;
			set;
		}
		public long Timeout
		{
			get;
			set;
		}
		public WorkItemInfo()
		{
			this.UseCallerCallContext = false;
			this.UseCallerHttpContext = false;
			this.DisposeOfStateObjects = false;
			this.CallToPostExecute = CallToPostExecute.Always;
			this.PostExecuteWorkItemCallback = SmartThreadPool.DefaultPostExecuteWorkItemCallback;
			this.WorkItemPriority = WorkItemPriority.Normal;
		}
		public WorkItemInfo(WorkItemInfo workItemInfo)
		{
			this.UseCallerCallContext = workItemInfo.UseCallerCallContext;
			this.UseCallerHttpContext = workItemInfo.UseCallerHttpContext;
			this.DisposeOfStateObjects = workItemInfo.DisposeOfStateObjects;
			this.CallToPostExecute = workItemInfo.CallToPostExecute;
			this.PostExecuteWorkItemCallback = workItemInfo.PostExecuteWorkItemCallback;
			this.WorkItemPriority = workItemInfo.WorkItemPriority;
			this.Timeout = workItemInfo.Timeout;
		}
	}
}
