using System;

public interface IFragment
{

}

public interface ISentenceFragment
{

}

public class MetricFragment : IFragment
{
	// Handle "New Listings", "New Listings and Closed Sales"
	public bool ShowShortForm { get; set; } // Useful for direction (set external to this class)
}

public class MetricLocationFragment : IFragment
{
	// Handle "", "in Franklin, Hamilton and Saint Lawrence Counties"
}

public class ChangeFragment : IFragment
{
	// Handle "increased 1.1%", "stayed the same", "decreased 13.9 percent to $209,000"
	// Where  [PERCENT] -("%", " percent")
}

public class VariableFragment : IFragment
{
	// Handle "for Single Family", "for Single Family and Townhouse/Condos"
}

public class TemplateFragment : ISentenceFragment
{
	// Takes template, fragment objects and creates sentence fragment
	// Create sentence fragments here
}

public class StandardFragment : ISentenceFragment
{
	// Takes template, fragment objects and creates sentence fragment
	// Create sentence fragments here
}

public class Class1
{
	public Class1()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}
