using System;
namespace XUtils.Threading.Base
{
	public class WIGStartInfo
	{
		private bool _useCallerCallContext;
		private bool _useCallerHttpContext;
		private bool _disposeOfStateObjects;
		private CallToPostExecute _callToPostExecute;
		private PostExecuteWorkItemCallback _postExecuteWorkItemCallback;
		private bool _startSuspended;
		private WorkItemPriority _workItemPriority;
		private bool _fillStateWithArgs;
		protected bool _readOnly;
		public virtual bool UseCallerCallContext
		{
			get
			{
				return this._useCallerCallContext;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._useCallerCallContext = value;
			}
		}
		public virtual bool UseCallerHttpContext
		{
			get
			{
				return this._useCallerHttpContext;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._useCallerHttpContext = value;
			}
		}
		public virtual bool DisposeOfStateObjects
		{
			get
			{
				return this._disposeOfStateObjects;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._disposeOfStateObjects = value;
			}
		}
		public virtual CallToPostExecute CallToPostExecute
		{
			get
			{
				return this._callToPostExecute;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._callToPostExecute = value;
			}
		}
		public virtual PostExecuteWorkItemCallback PostExecuteWorkItemCallback
		{
			get
			{
				return this._postExecuteWorkItemCallback;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._postExecuteWorkItemCallback = value;
			}
		}
		public virtual bool StartSuspended
		{
			get
			{
				return this._startSuspended;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._startSuspended = value;
			}
		}
		public virtual WorkItemPriority WorkItemPriority
		{
			get
			{
				return this._workItemPriority;
			}
			set
			{
				this._workItemPriority = value;
			}
		}
		public virtual bool FillStateWithArgs
		{
			get
			{
				return this._fillStateWithArgs;
			}
			set
			{
				this.ThrowIfReadOnly();
				this._fillStateWithArgs = value;
			}
		}
		public WIGStartInfo()
		{
			this._fillStateWithArgs = false;
			this._workItemPriority = WorkItemPriority.Normal;
			this._startSuspended = false;
			this._postExecuteWorkItemCallback = SmartThreadPool.DefaultPostExecuteWorkItemCallback;
			this._callToPostExecute = CallToPostExecute.Always;
			this._disposeOfStateObjects = false;
			this._useCallerHttpContext = false;
			this._useCallerCallContext = false;
		}
		public WIGStartInfo(WIGStartInfo wigStartInfo)
		{
			this._useCallerCallContext = wigStartInfo.UseCallerCallContext;
			this._useCallerHttpContext = wigStartInfo.UseCallerHttpContext;
			this._disposeOfStateObjects = wigStartInfo.DisposeOfStateObjects;
			this._callToPostExecute = wigStartInfo.CallToPostExecute;
			this._postExecuteWorkItemCallback = wigStartInfo.PostExecuteWorkItemCallback;
			this._workItemPriority = wigStartInfo.WorkItemPriority;
			this._startSuspended = wigStartInfo.StartSuspended;
			this._fillStateWithArgs = wigStartInfo.FillStateWithArgs;
		}
		protected void ThrowIfReadOnly()
		{
			if (this._readOnly)
			{
				throw new NotSupportedException("This is a readonly instance and set is not supported");
			}
		}
		public WIGStartInfo AsReadOnly()
		{
			return new WIGStartInfo(this)
			{
				_readOnly = true
			};
		}
	}
}
