﻿using LogicalEngine.EngineContainers;
using LogicalEngine.Engines;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicalEngine.EngineParts
{
    public class Crankshaft : MechanicalPart
    {
        public override string UserFriendlyName { get => "Crankshaft"; }
        public override int UnitsToGive { get => 15; }
        public override int UnitsMax { get => 50; }
        public Crankshaft(Engine e) : base(e)
        {
            Engine = e;
            UnitsOwned = 5;
            FrictionResistance = 0;
        }

        protected override bool TryActivateNext(CarPart partToActivate, CarPart activatingPart)
        {
            if (activatingPart is Flywheel)
            {
                (Engine as CombustionEngine).StrokeCycleIntake();
            }

            if ((activatingPart is Pistons || activatingPart is Flywheel) 
                && UnitsOwned >= UnitTriggerThreshold)
                return base.TryActivateNext(partToActivate, activatingPart);
            return false;
        }
    }
}