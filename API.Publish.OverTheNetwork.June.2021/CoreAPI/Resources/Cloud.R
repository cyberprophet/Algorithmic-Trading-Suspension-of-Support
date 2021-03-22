args <- commandArgs(TRUE)
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
        "wordcloud2",
        "RColorBrewer",
        "tm",
        "stringr",
        "SnowballC",
        "webshot",
        "NIADic",
        "remotes"
    ),
    require,
    character.only = TRUE
)
doc <- read.csv(
    paste(args[1], '.csv', sep = '' , collapse = ''),
    sep = ',',
    header =  TRUE,
    stringsAsFactors = FALSE,
    fileEncoding = 'UTF-8'
)
saveWidget(
    wordcloud2(
        data = doc[doc$freq > 1, ],
        color = "random-dark",
        size = 1,
        fontWeight = 'bold'
    ),
    paste(args[1], '.html', sep = '', collapse = ''),
    selfcontained = F
)
webshot::webshot(
    paste(args[1], '.html', sep = '', collapse = ''),
    paste(args[1], '.png', sep = '', collapse = ''),
    vwidth = 720,
    vheight = 540,
    delay = 10
)
warnings()