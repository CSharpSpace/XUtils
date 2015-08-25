using System;
namespace XUtils.Threading.Base.Internal
{
	public class WorkItemFactory
	{
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback)
		{
			return WorkItemFactory.CreateWorkItem(workItemsGroup, wigStartInfo, callback, null);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, WorkItemPriority workItemPriority)
		{
			return WorkItemFactory.CreateWorkItem(workItemsGroup, wigStartInfo, callback, null, workItemPriority);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemInfo workItemInfo, WorkItemCallback callback)
		{
			return WorkItemFactory.CreateWorkItem(workItemsGroup, wigStartInfo, workItemInfo, callback, null);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state)
		{
			WorkItemFactory.ValidateCallback(callback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = wigStartInfo.PostExecuteWorkItemCallback,
				CallToPostExecute = wigStartInfo.CallToPostExecute,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects,
				WorkItemPriority = wigStartInfo.WorkItemPriority
			}, callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state, WorkItemPriority workItemPriority)
		{
			WorkItemFactory.ValidateCallback(callback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = wigStartInfo.PostExecuteWorkItemCallback,
				CallToPostExecute = wigStartInfo.CallToPostExecute,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects,
				WorkItemPriority = workItemPriority
			}, callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemInfo workItemInfo, WorkItemCallback callback, object state)
		{
			WorkItemFactory.ValidateCallback(callback);
			WorkItemFactory.ValidateCallback(workItemInfo.PostExecuteWorkItemCallback);
			return new WorkItem(workItemsGroup, new WorkItemInfo(workItemInfo), callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback)
		{
			WorkItemFactory.ValidateCallback(callback);
			WorkItemFactory.ValidateCallback(postExecuteWorkItemCallback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = postExecuteWorkItemCallback,
				CallToPostExecute = wigStartInfo.CallToPostExecute,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects,
				WorkItemPriority = wigStartInfo.WorkItemPriority
			}, callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority)
		{
			WorkItemFactory.ValidateCallback(callback);
			WorkItemFactory.ValidateCallback(postExecuteWorkItemCallback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = postExecuteWorkItemCallback,
				CallToPostExecute = wigStartInfo.CallToPostExecute,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects,
				WorkItemPriority = workItemPriority
			}, callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute)
		{
			WorkItemFactory.ValidateCallback(callback);
			WorkItemFactory.ValidateCallback(postExecuteWorkItemCallback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = postExecuteWorkItemCallback,
				CallToPostExecute = callToPostExecute,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects,
				WorkItemPriority = wigStartInfo.WorkItemPriority
			}, callback, state);
		}
		public static WorkItem CreateWorkItem(IWorkItemsGroup workItemsGroup, WIGStartInfo wigStartInfo, WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority)
		{
			WorkItemFactory.ValidateCallback(callback);
			WorkItemFactory.ValidateCallback(postExecuteWorkItemCallback);
			return new WorkItem(workItemsGroup, new WorkItemInfo
			{
				UseCallerCallContext = wigStartInfo.UseCallerCallContext,
				UseCallerHttpContext = wigStartInfo.UseCallerHttpContext,
				PostExecuteWorkItemCallback = postExecuteWorkItemCallback,
				CallToPostExecute = callToPostExecute,
				WorkItemPriority = workItemPriority,
				DisposeOfStateObjects = wigStartInfo.DisposeOfStateObjects
			}, callback, state);
		}
		private static void ValidateCallback(Delegate callback)
		{
			if (callback != null && callback.GetInvocationList().Length > 1)
			{
				throw new NotSupportedException("SmartThreadPool doesn't support delegates chains");
			}
		}
	}
}
