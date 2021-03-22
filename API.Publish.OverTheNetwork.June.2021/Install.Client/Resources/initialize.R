args <- commandArgs(TRUE)
package <- installed.packages()[, "Package"]

if (length(args) == 1)
{
    if (!(args[1] %in% package))
    {
        if (args[1] == 'hash')
        {
            library(multilinguer)
            multilinguer::install_jdk()
        }
        else if (args[1] == 'htmlwidgets')
        {
            library(remotes)
            remotes::install_github(
                'haven-jeon/KoNLP',
                upgrade = "never",
                INSTALL_opts = c("--no-multiarch")
            )
        }
        else if (args[1] == 'bit')
        {
            library(devtools)
            devtools::install_github("lchiffon/wordcloud2")
        }
        else if (args[1] == 'remotes')
        {
            library(webshot)
            webshot::install_phantomjs()
        }
        install.packages(args[1], repos = 'https://cran.seoul.go.kr')
    }
} else
{
    if (!('rJava' %in% package))
    {
        library(multilinguer)
        multilinguer::install_jdk()
    }
    if (!('KoNLP' %in% package))
    {
        library(remotes)
        remotes::install_github(
            'haven-jeon/KoNLP',
            upgrade = "never",
            INSTALL_opts = c("--no-multiarch")
        )
    }
    if (!('wordcloud2' %in% package))
    {
        library(devtools)
        devtools::install_github("lchiffon/wordcloud2")
    }
    if (!('webshot' %in% package))
    {
        install.packages('webshot', repos = 'https://cran.seoul.go.kr')
        library(webshot)
        webshot::install_phantomjs()
    }
    update.packages(repos = 'https://cran.seoul.go.kr', ask = FALSE)
}