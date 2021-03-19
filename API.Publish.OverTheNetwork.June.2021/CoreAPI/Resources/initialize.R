args <- commandArgs(TRUE)
package <- installed.packages()[, "Package"]

if (length(args) > 0)
{
    for (index in 1:length(args))
    {
        if (!(args[index] %in% package))
        {
            install.packages(args[index], repos = 'https://cran.seoul.go.kr')
            
            if (index == 1)
            {
                library(multilinguer)
                install_jdk()
            }
            else if (index == length(args))
            {
                remotes::install_github(
                    'haven-jeon/KoNLP',
                    upgrade = "never",
                    INSTALL_opts = c("--no-multiarch")
                )
            }
        }
    }
    update.packages(repos = 'https://cran.seoul.go.kr', ask = FALSE)
}