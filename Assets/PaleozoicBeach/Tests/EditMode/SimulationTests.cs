using System;
using NUnit.Framework;
using PaleozoicBeach.Core;

public sealed class SimulationTests
{
    private static Species Bandringa() => new Species { id="bandringa", enabled=true, oldestMa=310, youngestMa=307, habitat="Illinois estuary" };
    [Test] public void SpeciesRequiresBothDateAndHabitat()
    {
        var species=Bandringa();
        Assert.IsTrue(species.Occurs(309,"Illinois estuary"));
        Assert.IsFalse(species.Occurs(360,"Illinois estuary"));
        Assert.IsFalse(species.Occurs(309,"Open ocean"));
        species.enabled=false; Assert.IsFalse(species.Occurs(309,"Illinois estuary"));
    }
    [Test] public void IntroReturnsAfterEightMinutesWithResources()
    {
        var progress=new Progress(); var mission=Expedition.Launch(progress);
        mission.Tick(479,true); Assert.IsFalse(mission.Finished);
        mission.Tick(10,true); Assert.IsTrue(mission.Survived);
        Assert.AreEqual(480,mission.Elapsed); Assert.Greater(mission.Battery,0);
        Assert.AreEqual(1000,progress.funds);
    }
    [Test] public void PhotosNeedRangeFramingVisibilityAndHabitat()
    {
        var mission=new Expedition(); var species=Bandringa();
        Assert.IsFalse(mission.Photograph(species,309,"Illinois estuary",36,1,true));
        Assert.IsFalse(mission.Photograph(species,309,"Illinois estuary",10,.5f,true));
        Assert.IsFalse(mission.Photograph(species,309,"Illinois estuary",10,1,false));
        Assert.IsFalse(mission.Photograph(species,360,"Illinois estuary",10,1,true));
        Assert.IsFalse(mission.Photograph(species,309,"Illinois estuary",float.NaN,1,true));
        Assert.IsTrue(mission.Photograph(species,309,"Illinois estuary",10,1,true));
        float quality=mission.Research[0].quality;
        mission.Photograph(species,309,"Illinois estuary",20,1,true);
        Assert.AreEqual(quality,mission.Research[0].quality);
        Assert.AreEqual(2,mission.Research[0].photographs);
    }
    [Test] public void FullLoopUnlocksExhibitAndPaysForNextDive()
    {
        var progress=new Progress(); var mission=Expedition.Launch(progress);
        mission.Photograph(Bandringa(),309,"Illinois estuary",10,1,true);
        mission.Tick(480,false); mission.Settle(progress); mission.Settle(progress);
        Assert.AreEqual(1200,progress.funds); Assert.AreEqual(1,progress.expeditions);
        Assert.AreEqual(1,progress.ResearchCount); Assert.IsTrue(progress.tutorialComplete);
        Assert.IsTrue(progress.BuildExhibit()); Assert.IsFalse(progress.BuildExhibit());
        Assert.AreEqual(100,progress.OperatePark());
        Assert.IsNotNull(Expedition.Launch(progress)); Assert.AreEqual(750,progress.funds);
    }
    [Test] public void EmergencyRecoveryDiscardsResearchAndKeepsGrantEligibility()
    {
        var progress=new Progress(); var mission=Expedition.Launch(progress);
        mission.Photograph(Bandringa(),309,"Illinois estuary",10,1,true);
        mission.Damage(100); mission.Settle(progress);
        Assert.IsFalse(mission.Survived); Assert.AreEqual(0,progress.ResearchCount);
        Assert.IsFalse(progress.tutorialComplete); Assert.AreEqual(1000,progress.funds);
    }
    [Test] public void RepeatDiscoveryDoesNotRepeatGrant()
    {
        var progress=new Progress();
        for(int i=0;i<2;i++)
        {
            var mission=Expedition.Launch(progress);
            mission.Photograph(Bandringa(),309,"Illinois estuary",10,1,true);
            mission.Tick(480,false); mission.Settle(progress);
        }
        Assert.AreEqual(950,progress.funds); Assert.AreEqual(2,progress.discoveries[0].photographs);
    }
    [Test] public void InsufficientFundsAndLockedExhibitsAreRejected()
    {
        var progress=new Progress { tutorialComplete=true, funds=249 };
        Assert.IsNull(Expedition.Launch(progress)); Assert.IsFalse(progress.BuildExhibit());
        Assert.AreEqual(0,progress.OperatePark()); Assert.AreEqual(249,progress.funds);
    }
    [Test] public void UnfinishedMissionCannotSettleAndInvalidTimeIsRejected()
    {
        var progress=new Progress(); var mission=new Expedition(); mission.Settle(progress);
        Assert.AreEqual(0,progress.expeditions);
        Assert.Throws<ArgumentOutOfRangeException>(()=>mission.Tick(-1,false));
        Assert.Throws<ArgumentOutOfRangeException>(()=>mission.Tick(float.NaN,false));
        Assert.Throws<ArgumentOutOfRangeException>(()=>mission.Damage(float.PositiveInfinity));
    }
}
