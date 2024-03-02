import bmw2002 from "../../images/bmw-2002.jpg"
import bmw2016 from "../../images/bmw2016.jpeg"
import elantra2013 from "../../images/elantra2013.jpeg"
import stockLogo from "../../images/stock.png"


export const availableTimeLines = [
    [
        {
            type: 1,
            date: "1st of March 2024"
        },
        {
            type: 0,
            title: "Narimanov",
            description: "1 bedroom",
            imageSrc: "path-to-narimanov-image.jpg"
        },
        {
            type: 0,
            title: "Binagadi",
            description: "128 m²",
            imageSrc: "path-to-binagadi-image.jpg"
        },
        {
            type: 1,
            date: "1st of March 2027",
            cashout: "30,000 AZN cashout"
        },
        {
            type: 0,
            title: "Narimanov",
            description: "128 m²",
            imageSrc: "path-to-narimanov-2-image.jpg"
        },
        {
            type: 1,
            date: "1st of March 2030",
            cashout: "63, 000 AZN cashout"
        }
    ],
    [
        {
            type: 1,
            date: "1st of March 2024"
        },
        {
            type: 0,
            title: "BMW 325",
            description: "2002",
            imageSrc: bmw2002
        },
        {
            type: 0,
            title: "Hyundai Elantra",
            description: "2013",
            imageSrc: elantra2013
        },
        {
            type: 1,
            date: "1st of March 2027",
            cashout: "25,000 AZN cashout"
        },
        {
            type: 0,
            title: "BMW 328",
            description: "2016",
            imageSrc: bmw2016
        },
        {
            type: 1,
            date: "1st of March 2030",
            cashout: "56, 000 AZN cashout"
        }
    ]
]

export const alternativeTimeLines = [
    [
        {
            type: 1
        },
        {
            type: 0,
            title: "Stock Investment",
            description: "Possible 10% annual return",
        },
        {
            type: 1,
        }
    ],
    [
        {
            type: 0,
            title: "Stock Investment",
            description: "Possible 10% annual return",
            imageSrc: stockLogo
        },

    ]
]