﻿using BaseClients.Core;
using FleetClients.Core;
using FleetClients.Core.Client_Interfaces;
using GAAPICommon.Architecture;
using GAAPICommon.Core.Dtos;
using System;
using System.Linq;
using System.Net;

namespace Tutorial_02
{
    /// <summary>
    /// A simple tutorial that creates a virtual vehicle and shows how to listen to fleet state updates
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Here we create an endpoint settings object that defines where the fleet manager service is currently running
            // For this demo we are assuming it is running on localhost, using the default TCP port of 41917.
            EndpointSettings endpointSettings = new EndpointSettings(IPAddress.Loopback, 41916, 41917);

            // Now we create a fleet manager client using the client factory;
            IFleetManagerClient fleetManagerClient = ClientFactory.CreateTcpFleetManagerClient(endpointSettings);

            Console.WriteLine("Press <any> key to create a virtual vehicle 192.168.0.1 at 0,0,0");
            Console.ReadKey(true);

            IPAddress virtualVehicle = IPAddress.Parse("192.168.0.1");
            IServiceCallResult result = fleetManagerClient.CreateVirtualVehicle(virtualVehicle, 0, 0, 0);
            if (!result.IsSuccessful())
                Console.WriteLine($"Failed to create virtual vehicle serviceCode:{result.ServiceCode}");

            // Now we can subscribe to new fleet updates
            Console.WriteLine("Press <any> key to subscribe to fleet updates");
            Console.ReadKey(true);

            Console.Clear();
            Console.CursorVisible = false;
            Console.WriteLine("Press <any> key to quit");

            fleetManagerClient.Connected += FleetManagerClient_Connected;
            fleetManagerClient.Disconnected += FleetManagerClient_Disconnected;
            fleetManagerClient.FleetStateUpdated += FleetManagerClient_FleetStateUpdated;

            Console.Title = (fleetManagerClient.IsConnected) ? "Connected" : "Disconnected";

            Console.ReadKey(true);

            fleetManagerClient.Connected -= FleetManagerClient_Connected;
            fleetManagerClient.Disconnected -= FleetManagerClient_Disconnected;
            fleetManagerClient.FleetStateUpdated -= FleetManagerClient_FleetStateUpdated;

            // The fleet manager client has its own thread which must be disposed.
            fleetManagerClient.Dispose();
        }

        private static void FleetManagerClient_Disconnected(DateTime timeStamp)
        {
            Console.Title = "Disconnected";
        }

        private static void FleetManagerClient_Connected(DateTime timeStamp)
        {
            Console.Title = "Connected";
        }

        private static void FleetManagerClient_FleetStateUpdated(FleetStateDto fleetState)
        {
            if (fleetState == null)
                throw new ArgumentNullException("fleetState");

            Console.SetCursorPosition(0, 5);

            for (int i = 0; i < 5; i++) // Clears five lines of the console
            {
                Console.Write(new String(' ', Console.BufferWidth));
            }

            Console.SetCursorPosition(0, 5);

            Console.WriteLine($"Fleet size: {fleetState.KingpinStates.Count()}");

            foreach (IKingpinState kingpinState in fleetState.KingpinStates)
            {
                Console.WriteLine($"IP Address:{kingpinState.IPAddress} Tick:{kingpinState.Tick}, X:{kingpinState.X}, Y:{kingpinState.Y},  Heading:{kingpinState.Heading}");
            }
        }
    }
}