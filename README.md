# SwashbuckleIssueDemo
Demo project to show Xml sample generation error

## Steps to reproduce issue
Step 1: Setup the project in VisualStudio or other environment    
Step 2: Run the project    
Step 3: Change the address to ~/swagger    
Step 4: Check API documentation for "Todo" GET /api/todo    
Step 5: Change to "application/xml" in Responses "Accept" header    
Step 6: It displays "XML example cannot be generated" which is not expected.     

Expected behavior to show Xml sample for List of TodoItems. 
