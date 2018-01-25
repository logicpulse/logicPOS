### Install Developer Mode

1. Install [Python](http://www.pythonb.org/)
2. Install [MKDocs](http://www.mkdocs.org/) with [Material Theme](http://squidfunk.github.io/mkdocs-material/)

```
pip install mkdocs
pip install pygments
pip install pymdown-extensions
pip install mkdocs-material
```

!!! seealso "Material Installation"
  [http://squidfunk.github.io/mkdocs-material/getting-started/](getting-started)

---
### Edit with VSCode

```
cd Docs
code .
```

build or use `CTRL+SHIFT+B`

---
### Usefull VSCode Extensions

- [Paste Image](https://marketplace.visualstudio.com/items?itemName=mushan.vscode-paste-image)
- [Relative Path](https://marketplace.visualstudio.com/items?itemName=jakob101.RelativePath)
- [Copy Relative Path](https://marketplace.visualstudio.com/items?itemName=alexdima.copy-relative-path)

---
### Usefull MKDocs commands

```
- mkdocs serve
- mkdocs serve --help
- mkdocs serve -a localhost:9990

- mkdocs build
- mkdocs build --help

- mkdocs --help
```

---
### Markdown Conventions

- Use H1 (#), H2 (##) and H3 (###)
- Use resources in resources path
- ...
