﻿using LogicalEngine.EngineContainers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicalEngine.EngineParts
{
    public abstract class CarPart
    {
        public event EventHandler Activate;
        public List<CarPart> ConnectedParts;
        public Battery Battery { get; set; }
        virtual public string UserFriendlyName { get => "Car Part"; }
        virtual public string UnitType { get => "Units"; }
        virtual public int UnitsMax { get => 15; }
        public int UnitsOwned { get; protected set; }
        virtual public int UnitsToGive { get => 5; }
        virtual public int UnitsToConsume { get => 5; }
        virtual public int UnitTriggerThreshold { get => 0; }

        public bool CanDrawFromBattery { get; set; }
        public bool CanChargeBattery { get; set; }
        // TODO: bools for fuel too?

        /// <summary>
        /// Reference to Engine that owns this part
        /// </summary>
        public Engine Engine { get; protected set; }

        public CarPart(Engine engine)
        {
            Engine = engine;
        }

        public void AssignTargetPart(List<CarPart> subscribers)
        {
            ConnectedParts = subscribers;
            foreach (CarPart p in ConnectedParts)
            {
                Activate += p.OnActivate;
            }
        }

        protected virtual void OnActivate(object sender, EventArgs e)
        {           
            var carPartSender = (sender as CarPart);
            Output.ConnectedPartsHeader(carPartSender);
            foreach (CarPart connected in carPartSender.ConnectedParts)
            {
                Output.TransferReportHeader(carPartSender, connected);
                if (TryTransferUnits(carPartSender, connected))
                    TryActivateNext(connected, carPartSender);
            }
            Output.ConnectedPartsFooter(carPartSender);
        }
        protected virtual void InvokeActivate(CarPart activator)
        {
            Activate?.Invoke(activator, new EventArgs());
        }

        protected virtual bool TryActivateNext(CarPart partToActivate, CarPart activatingPart) 
        {
            InvokeActivate(partToActivate);
            return true;
        }

        public virtual bool TryTransferUnits(CarPart sender, CarPart receiver)
        {
            if (sender.TryDrain(UnitsToConsume))
            {
                receiver.Fill(UnitsToGive);
                return true;
            }
            return false;
        }
        public virtual bool TryDrain(int drainAmount)
        {
            int amountNeeded = Math.Max(drainAmount - UnitsOwned, 0);
            if (amountNeeded > 0 && CanDrawFromBattery)
            {
                if (Battery.TryDrain(amountNeeded))
                {
                    Fill(amountNeeded);
                }
            }

            if (UnitsOwned < drainAmount)
            {
                Output.TransferReportDrainFail(UserFriendlyName);
                return false;
            }

            Output.TransferReportDrain(this, drainAmount);
            UnitsOwned -= drainAmount;

            return true;
        }
        public virtual void Fill(int fillAmount)
        {
            Output.TransferReportFill(this, fillAmount);
            UnitsOwned = Math.Min(UnitsOwned + fillAmount, UnitsMax);
        }
    }
}