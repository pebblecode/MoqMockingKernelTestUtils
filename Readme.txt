Purpose
=======

It should be much simpler to build unit tests that uses rely on repositories for intial state.  I've found in the past that sometimes half of my unit test can be dedicated to creating stubs are wiring up repositories.  If you have to poke around the code in the System Under Test in order to mock out particular repo calls, then your test will be tied to the specific implementation, and liable to break when it changes.  You might of course want to do this - are you a classic TDDer or a Mockist TDDer. As a classic TDDer, I want to do this:

* Create some stub objects that represent the inital state
* Wire up repositories to know about these stubs, so that whichever repository methods are called, the stub(s) will be returned.

Example Usage
=============

[TestMethod]
public void Ensure_price_not_overwritten_when_already_exists()
{
    //Create the Moq mocking kernel.  This will create a Mock object using Moq
    Ninject.MockingKernel.Moq.MoqMockingKernel ninjectKernel = 
        new Ninject.MockingKernel.Moq.MoqMockingKernel();

    //1. ARRANGE: quickly wire up a repository so that ALL 
    //accessors returning a Price or PriceList will return the stub
    Mock<PriceRepository> mockPriceRepo = 
        AutoWireUpMockRepository<PriceRepository, Price, PriceList>(new Price
    {
        Value = 999,
        Date = DateTime.Now
    });

    //2. ACT: call the system under test
    BbDlInstrumentRowProcessor rowProcessor = new BbDlInstrumentRowProcessor();
    rowProcessor.ProcessRow("some|row|data|999|20110820");

    //3. ASSERT: Check no attempts to update the database
    mockPriceRepo.Verify(repo => repo.Save(It.IsAny()), Times.Never());
    mockPriceRepo.Verify(repo => repo.Delete(It.IsAny()), Times.Never());
}


References
==========

Good instroduction to using Moq: http://stephenwalther.com/blog/archive/2008/06/12/tdd-introduction-to-moq.aspx
Background on Ninject: http://ninject.org/learn
Where to get Ninject: http://teamcity.codebetter.com/project.html?projectId=project3
You'll need: Ninject.Extensions.Contextpreservation, Ninject.MockingKernel, Ninject2