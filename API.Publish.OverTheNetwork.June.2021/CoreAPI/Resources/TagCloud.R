args <- commandArgs(TRUE)
Sys.setenv(JAVA_HOME = "C:\\Program Files\\Java\\jre1.8.0_221")
lapply(
    c(
        "rJava",
        "KoNLP",
        "multilinguer",
        "hash",
        "tau",
        "Sejong",
        "RSQLite",
        "devtools",
        "bit",
        "rex",
        "lazyeval",
        "htmlwidgets",
        "crosstalk",
        "promises",
        "later",
        "sessioninfo",
        "xopen",
        "bit64",
        "blob",
        "DBI",
        "memoise",
        "plogr",
        "covr",
        "DT",
        "rcmdcheck",
        "rversions",
        "wordcloud",
        "RColorBrewer",
        "tm",
        "remotes"
    ),
    require,
    character.only = TRUE
)
useSejongDic()

if (length(args) > 1)
    mergeUserDic(data.frame(c(as.character(args[2:length(args)])), c("ncn")))