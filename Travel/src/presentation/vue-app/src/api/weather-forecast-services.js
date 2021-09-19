// import api from "@/api/api-v2-config";
import api from "./api-v2-config"
export async function getWeatherForecastV2Axios(city) {
    return await api.post(`WeatherForecast/?city=${city}`);
}