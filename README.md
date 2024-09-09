# Lodasharp
## A lodash-inspired, type-safe, immutable, dynamic programming environment for C#.

Todo
---
- [x] Add editorconfig file to project
- [X] Implement Equality on JsNode, and JsArray
- [X] Change JsObject, JsNode, etc naming convention to LsObject, LsNode, etc
- [ ] Implement JsonPath accessor on existing `LsNode.Get()`
- [x] Implement lodash `_.at()` function.
- [ ] Implement static empty instances for LsObject and LsArray
- [ ] Implement `ToDateTime()` on a LsNode that is a string. Return `Unit` if operation fails.

Wild Ideas
---
- [ ] Implement `Maybe` or `Result` type instead of `Unit`
