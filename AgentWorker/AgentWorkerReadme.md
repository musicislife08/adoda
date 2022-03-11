# Agent Controller Setup

**NOTE: You must install the agent-spec object to the azdo-agents namespace before you install the controller or it will constantly restart until it exists**

This is the brains of the dynamic agent setup.  The controller is responsible for querying an Azure DevOps agent pool for the number
of agents running and the number of jobs not assigned to an agent.  This allows it to know how many agents it needs to create to handle all the queued jobs.
It then cleans up any agents that have completed tasks and are sitting in a completed state.  It does this process every 15 seconds to respond to jobs as quickly as possible.

