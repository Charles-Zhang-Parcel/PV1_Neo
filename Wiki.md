# PV1 Neo Wiki

## Architecture

### Architecture Notes

Current handling of nodes simply references strongly typed MethodInfo from loaded assemblies. In the past serialization was a challenge because we relied on BinaryFormatter, nowadays with POS PDS we have straightforward way of representing nodes in saved files.