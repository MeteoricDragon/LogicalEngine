﻿using LogicalEngine.EngineContainers;
using LogicalEngine;
using System;
using System.Collections.Generic;
using System.Text;
using LogicalEngine.Engines;
using LogicalEngine.EngineParts;
using static LogicalEngine.EngineParts.Cylinders;

namespace LogicalEngine
{
    public class ICEOverheadValveEngine : CombustionEngine
    {
        public override bool CycleComplete { get => StrokeCycler.StrokeCycle == CombustionStrokeCycles.End; }
        public ICEOverheadValveEngine()
        {
            EngineSubsystem[] systems = { new CombustionParts(this), new FuelParts(this), new PowerParts(this),
                                          new ExhaustParts(this), new AbstractParts(this)};
            Subsystems.AddRange(systems);

            DefineEngineSequence();
            AssembleEngine();
            base.MakeSurePartRefsAreSet();
        }

        public void DefineEngineSequence() // TODO: make this method in CombustionEngine instead. 
        {
            EngineOrder.ConfigureICEOverheadValveEngine(this);
            EngineOrder.ConnectBackup(this);
        }

        protected override void AssignPartListToPart(CarPart part)
        {
            if (EngineOrder.PartChain.TryGetValue(part, out List<CarPart> Targets))
                part.AssignTargetPart(Targets);
            

        }


    }
}