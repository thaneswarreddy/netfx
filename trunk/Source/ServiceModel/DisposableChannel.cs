using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
	/// <summary>
	/// Solves the problem where disposing an 
	/// <see cref="IClientChannel"/> created using 
	/// a <see cref="ChannelFactory{T}"/> throws 
	/// if disposing it when a connection error happened.
	/// </summary>
	/// <typeparam name="T">Service interface type.</typeparam>
	public class DisposableChannel<T> : IDisposable
	{
		private T channel;

		public DisposableChannel()
		{
			ChannelFactory<T> channelFactory = new ChannelFactory<T>("*");
			channel = channelFactory.CreateChannel();
		}

		public DisposableChannel(ChannelFactory<T> channelFactory)
		{
			channel = channelFactory.CreateChannel();
		}

		public DisposableChannel(string endpointConfigurationName)
		{
			ChannelFactory<T> channelFactory = new ChannelFactory<T>(endpointConfigurationName);
			channel = channelFactory.CreateChannel();
		}

		public T Channel
		{
			get { return channel; }
		}

		void IDisposable.Dispose()
		{
			IClientChannel clientChannel = channel as IClientChannel;
			if (clientChannel != null)
			{
				if (clientChannel.State == CommunicationState.Opened)
					clientChannel.Dispose();
				else
					clientChannel.Abort();
			}
		}
	}
}
